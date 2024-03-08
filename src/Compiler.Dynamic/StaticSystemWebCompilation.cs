// MIT License.

using System.Reflection;
using System.Text.Json;
using System.Web;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static WebForms.Compiler.Dynamic.StaticSystemWebCompilation;

namespace WebForms.Compiler.Dynamic;

internal sealed class StaticSystemWebCompilation : SystemWebCompilation<PersistedCompiledPage>, IWebFormsCompiler
{
    private readonly IOptions<StaticCompilationOptions> _options;
    private readonly ILogger _logger;

    public StaticSystemWebCompilation(
        IOptions<StaticCompilationOptions> options,
        IOptions<WebFormsOptions> webFormsOptions,
        IOptions<PageCompilationOptions> pageCompilationOptions,
        IMetadataProvider metadataProvider,
        ILoggerFactory factory)
        : base(factory, metadataProvider, webFormsOptions, pageCompilationOptions)
    {
        _logger = factory.CreateLogger<StaticSystemWebCompilation>();
        _options = options;
    }

    protected override PersistedCompiledPage CreateCompiledPage(
        Compilation compilation,
        string route,
        string typeName,
        IEnumerable<SyntaxTree> trees,
        IEnumerable<MetadataReference> references,
        IEnumerable<EmbeddedText> embedded,
        IEnumerable<Assembly> assemblies,
        CancellationToken token)
    {
        // TODO: Handle output better
        using (var peStream = File.OpenWrite(Path.Combine(_options.Value.TargetDirectory, $"WebForms.{typeName}.dll")))
        {
            using var pdbStream = File.OpenWrite(Path.Combine(_options.Value.TargetDirectory, $"WebForms.{typeName}.pdb"));
            peStream.SetLength(0);
            pdbStream.SetLength(0);

            var result = compilation.Emit(
                embeddedTexts: embedded,
                peStream: peStream,
                pdbStream: pdbStream,
                cancellationToken: token);

            if (!result.Success)
            {
                _logger.LogError("{ErrorCount} error(s) found compiling {Route}", result.Diagnostics.Length, route);

                var errors = GetErrors(result.Diagnostics).ToList();
                var errorResult = JsonSerializer.Serialize(errors);

                File.WriteAllText(Path.Combine(_options.Value.TargetDirectory, $"{typeName}.errors.json"), errorResult);

                throw new RoslynCompilationException(route, errors);
            }
        }

        var assembly = Assembly.LoadFile(Path.Combine(_options.Value.TargetDirectory, $"WebForms.{typeName}.dll"));

        if (assembly.GetType(typeName) is Type type)
        {
            return new PersistedCompiledPage(new(route), embedded.Select(t => t.FilePath).ToArray())
            {
                TypeName = typeName,
                Assembly = $"WebForms.{typeName}",
                MetadataReference = compilation.ToMetadataReference(),
                Type = type,
            };
        }

        throw new InvalidOperationException("No type found");
    }

    Task IWebFormsCompiler.CompilePagesAsync(CancellationToken token)
    {
        Environment.CurrentDirectory = _options.Value.InputDirectory;

        try
        {
            CompileAllPages(token);

            var pagesPath = Path.Combine(_options.Value.TargetDirectory, "webforms.pages.json");

            File.WriteAllText(pagesPath, JsonSerializer.Serialize(GetDetails(), new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception e)
        {
            _logger.LogError("Unexpected error: {Message} {Stacktrace}", e.Message, e.StackTrace);
        }

        return Task.CompletedTask;
    }

    private IEnumerable<PageDetails> GetDetails()
    {
        foreach (var page in GetAllCompiledObjects())
        {
            yield return new PageDetails(page.Path, page.TypeName, page.Assembly);
        }
    }

    protected override PersistedCompiledPage CreateErrorPage(string path, Exception e) => throw e;

    private sealed record PageDetails(string Path, string Type, string Assembly);

    internal sealed class PersistedCompiledPage : CompiledPage
    {
        public PersistedCompiledPage(PagePath path, string[] dependencies)
            : base(path, dependencies)
        {
        }

        public string TypeName { get; init; } = null!;

        public string Assembly { get; init; } = null!;

    }
}
