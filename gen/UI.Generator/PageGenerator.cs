﻿using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Threading;

namespace Microsoft.AspNetCore.SystemWebAdapters.UI.Generator;

[Generator]
public class PageGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var aspxFiles = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            .Select((text, token) => Parse(text, token));
        context.RegisterSourceOutput(aspxFiles, GenerateSource);
    }

    private void GenerateSource(SourceProductionContext context, ParsedPage page)
    {
        context.AddSource(page.Path, GetSource(page));
    }

    private string GetSource(ParsedPage page)
    {
        using var stringWriter = new StringWriter();

        using (var writer = new IndentedTextWriter(stringWriter))
        {
            writer.Write("public partial class ");
            writer.WriteLine(page.Name);
            writer.WriteLine("{");
            writer.WriteLine("}");
        }

        return stringWriter.ToString();
    }

    private ParsedPage Parse(AdditionalText text, CancellationToken token)
    {
        var path = text.Path.TrimStart('/') + ".g.cs";

        return new ParsedPage
        {
            Path = path,
            Name = "About",
        };
    }

    private class ParsedPage
    {
        public string Path { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}
