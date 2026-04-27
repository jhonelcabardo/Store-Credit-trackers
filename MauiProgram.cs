using Microsoft.Extensions.Logging;
using Store_Credit_Tracker.Services;
using Store_Credit_Tracker.Views;

namespace Store_Credit_Tracker;

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

        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<LedgerService>();

        builder.Services.AddTransient<Add>();
        builder.Services.AddTransient<AddUtangPage>();
        builder.Services.AddTransient<PaymentsPage>();
        builder.Services.AddTransient<CustomerStatementPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}