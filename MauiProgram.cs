using Blackjack.Services;
using Blackjack.ViewModels;
using Blackjack.Views;
using FluentIcons.Maui;
using Microsoft.Extensions.Logging;

namespace Blackjack
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseFluentIcons()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register ViewModels
            builder.Services.AddSingleton<MainMenuViewModel>();
            builder.Services.AddTransient<SeatSelectionViewModel>();
            builder.Services.AddTransient<GameTableViewModel>();

            // Register Views
            builder.Services.AddSingleton<MainMenuPage>();
            builder.Services.AddTransient<SeatSelectionPage>();
            builder.Services.AddTransient<GameTablePage>();

            // Register Services
            builder.Services.AddSingleton<BasicStrategy>();
            builder.Services.AddSingleton<GameRules>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
