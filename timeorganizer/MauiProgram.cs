using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using timeorganizer.Services;
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
            builder.Services.AddSingleton<ToDoService>();
            builder.Services.AddSingleton<SettingsService>();
            builder.Services.AddSingleton<ActivityService>();
            builder.Services.AddSingleton<RegisterService>();
            builder.Services.AddSingleton<ContactService>();
            builder.Services.AddAuthorizationCore();
            builder.Services.TryAddScoped<AuthenticationStateProvider, AuthService>();
            builder.Services.AddSingleton<AuthServiceSetUser>();


            builder.Services.AddOptions();
            return builder.Build();
        }
    }
}