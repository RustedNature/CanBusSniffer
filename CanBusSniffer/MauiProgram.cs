using CanBusSniffer.ViewModels;
using CanBusSniffer.Views;
using Microsoft.Extensions.Logging;

namespace CanBusSniffer
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
            builder.Services.AddSingleton<MainVM>();
            builder.Services.AddSingleton<MainCV>();
            builder.Services.AddSingleton<BluetoothConnectionVM>();
            builder.Services.AddSingleton<BluetoothPage>();
            builder.Services.AddSingleton<MainPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}