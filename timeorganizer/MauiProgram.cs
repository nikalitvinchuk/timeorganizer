using Microsoft.Extensions.Logging;
using timeorganizer.DatabaseModels;
using timeorganizer.Views;
using timeorganizer.PageViewModels;

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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseLogin>();
            builder.Services.AddSingleton<Users>();
            builder.Services.AddSingleton<RegisterPageViewModel>(); 
            builder.Services.AddSingleton<RegisterPage>();
            return builder.Build();
        }
    }
}