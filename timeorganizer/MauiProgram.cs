using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

using timeorganizer.Helpers;
using timeorganizer.PageViewModel;
using timeorganizer.PageViewModels;
using timeorganizer.Service;

namespace timeorganizer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<ActivityService>();
            builder.Services.AddSingleton<RegisterService>();
            builder.Services.AddSingleton<ContactService>();

            builder.Services.AddAuthorizationCore();
            return builder.Build();
        }
    }
}