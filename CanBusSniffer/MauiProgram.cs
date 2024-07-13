﻿using CanBusSniffer.Service;
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
            builder.Services.AddTransient<MainVM>();
            builder.Services.AddTransient<MainCV>();
            builder.Services.AddTransient<BluetoothConnectionVM>();
            builder.Services.AddTransient<BluetoothPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<BluetoothService>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}