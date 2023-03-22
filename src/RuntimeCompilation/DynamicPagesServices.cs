// MIT License.

using System.Web.UI;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.AspNetCore.SystemWebAdapters.UI.RuntimeCompilation;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class DynamicPagesServices
{
    public static ISystemWebAdapterBuilder AddDynamicPages(this ISystemWebAdapterBuilder services, Action<PageCompilationOptions> configure)
    {
        services.Services.AddSingleton<IPageCompiler, SystemWebCompilation>();
        services.Services.AddHostedService<WebFormsCompilationService>();

        services.Services.AddOptions<PageCompilationOptions>()
            .Configure(configure);

        services.Services.AddOptions<PagesSection>()
            .Configure<IOptions<PageCompilationOptions>>((options, compilation) =>
            {
                foreach (var known in compilation.Value.KnownTags)
                {
                    options.DefaultTagNamespaceRegisterEntries.Add(known);
                }

                options.EnableSessionState = System.Web.Configuration.PagesEnableSessionState.True;
            });

        return services;
    }
}
