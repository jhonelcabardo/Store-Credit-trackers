using Store_Credit_Tracker.Models;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.Views;

public partial class Add : ContentPage
{
    private readonly DatabaseService _databaseService;

    public Add(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var customer = new Customer
        {
            FullName = txtFullName.Text ?? "",
            PhoneNumber = txtPhone.Text ?? "",
            CustomerCode = $"CUST{DateTime.Now:yyyyMMddHHmmss}"
        };

        await _databaseService.AddCustomerAsync(customer);
        await DisplayAlertAsync("Success", "Customer added.", "OK");
        await Navigation.PopAsync();
    }
}