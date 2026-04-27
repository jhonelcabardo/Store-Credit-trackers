using Microsoft.Extensions.DependencyInjection;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.Views;

public partial class RemindersPage : ContentPage
{
    private readonly DatabaseService? _databaseService;

    public RemindersPage()
    {
        InitializeComponent();

        _databaseService = Application.Current?
            .Handler?
            .MauiContext?
            .Services
            .GetService<DatabaseService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            if (_databaseService == null)
            {
                await DisplayAlertAsync("Error", "DatabaseService is not available.", "OK");
                return;
            }

            var overdueCustomers = await _databaseService.GetOverdueCustomersAsync();
            OverdueCustomersView.ItemsSource = overdueCustomers;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to load reminders: {ex.Message}", "OK");
        }
    }
}