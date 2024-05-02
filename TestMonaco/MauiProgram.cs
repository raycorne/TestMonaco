using CodeEditor.Infrastructure;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace TestMonaco
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMudServices();
            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

            builder.Services.AddInfrastructureServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
