using Microsoft.Extensions.DependencyInjection;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.Views;

public partial class ReportsPage : ContentPage
{
    private readonly DatabaseService? _databaseService;

    public ReportsPage()
    {
        InitializeComponent();

        _databaseService = Application.Current?
            .Handler?
            .MauiContext?
            .Services
            .GetService<DatabaseService>();
    }

    private async void OnGenerateReportClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_databaseService == null)
            {
                await DisplayAlertAsync("Error", "DatabaseService is not available.", "OK");
                return;
            }

            var totalUtang = await _databaseService.GetTotalUtangAsync();
            var totalPaid = await _databaseService.GetTotalPaidAsync();
            var totalCustomers = await _databaseService.GetTotalCustomersAsync();

            TotalUtangLabel.Text = $"Total Utang: ₱{totalUtang:N2}";
            TotalPaidLabel.Text = $"Total Paid: ₱{totalPaid:N2}";
            NetBalanceLabel.Text = $"Net Balance: ₱{totalUtang - totalPaid:N2}";
            TransactionCountLabel.Text = $"Total Customers: {totalCustomers}";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to generate report: {ex.Message}", "OK");
        }
    }
}