using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LoggedInUserLabel.Text = string.IsNullOrWhiteSpace(App.LoggedInUser)
            ? "Admin"
            : App.LoggedInUser;

        var settings = await App.Database.GetSettingsAsync();

        DarkModeSwitch.IsToggled = settings.DarkModeEnabled;
        NotificationSwitch.IsToggled = settings.NotificationsEnabled;
        BackupSwitch.IsToggled = settings.AutoBackupEnabled;
    }

    private async void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        Application.Current!.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        await App.Database.UpdateDarkModeAsync(e.Value);

        await DisplayAlert(
            "Theme Changed",
            e.Value ? "Dark mode enabled." : "Light mode enabled.",
            "OK");
    }

    private async void OnNotificationToggled(object sender, ToggledEventArgs e)
    {
        await App.Database.UpdateNotificationsAsync(e.Value);

        await DisplayAlert(
            "Notifications",
            e.Value ? "Notifications enabled." : "Notifications disabled.",
            "OK");
    }

    private async void OnBackupToggled(object sender, ToggledEventArgs e)
    {
        await App.Database.UpdateAutoBackupAsync(e.Value);

        await DisplayAlert(
            "Auto Backup",
            e.Value ? "Auto backup enabled." : "Auto backup disabled.",
            "OK");
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Change Password", "This feature will be added soon.", "OK");
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "About App",
            "Store Credit Tracker with Sinking Fund module.\n\nVersion 1.0",
            "OK");
    }

    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Help & Support",
            "For support, contact the app administrator.",
            "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");

        if (answer)
        {
            App.LoggedInUser = string.Empty;
            await DisplayAlert("Logged Out", "You have been logged out.", "OK");
        }
    }
}