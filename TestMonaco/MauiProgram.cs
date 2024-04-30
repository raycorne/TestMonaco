using CodeEditor.Appl.Interfaces;
using CodeEditor.Infrastructure;
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
                //.UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMudServices();
            builder.Services.AddMauiBlazorWebView();


            builder.Services.AddInfrastructureServices();

#if WINDOWS
            builder.Services.AddTransient<IFolderPicker, Platforms.Windows.FolderPicker>();
            
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
