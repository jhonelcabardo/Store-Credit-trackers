using Store_Credit_Tracker.Services;
using Store_Credit_Tracker.Views;

namespace Store_Credit_Tracker;

public partial class App : Application
{
    public static string LoggedInUser { get; set; } = string.Empty;
    public static DatabaseService? Database { get; private set; }

    public App()
    {
        InitializeComponent();

        MainPage = new ContentPage
        {
            Content = new Label
            {
                Text = "Loading...",
                FontSize = 24,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        InitializeAppAsync();
    }

    private async void InitializeAppAsync()
    {
        try
        {
            Database = new DatabaseService();
            await Database.InitializeAsync();

            MainPage = new NavigationPage(new LoginPage());
        }
        catch (Exception ex)
        {
            MainPage = new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new Label
                    {
                        Text = ex.ToString(),
                        TextColor = Colors.Red,
                        Padding = 20,
                        FontSize = 14
                    }
                }
            };
        }
    }
}