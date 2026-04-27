using Store_Credit_Tracker.Models;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.Views;

public partial class AddUtangPage : ContentPage
{
    private readonly LedgerService _ledgerService;
    private Customer? selectedCustomer;

    public AddUtangPage(LedgerService ledgerService)
    {
        InitializeComponent();
        _ledgerService = ledgerService;
    }

    public void SetCustomer(Customer customer)
    {
        selectedCustomer = customer;
        lblCustomerName.Text = customer.FullName;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            if (selectedCustomer == null)
            {
                await DisplayAlertAsync("Error", "Please select a customer first.", "OK");
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out var amount))
            {
                await DisplayAlertAsync("Error", "Invalid amount.", "OK");
                return;
            }

            await _ledgerService.AddCreditAsync(
                selectedCustomer.Id,
                amount,
                dueDatePicker.Date,
                txtNotes.Text ?? "",
                App.LoggedInUser);

            await DisplayAlertAsync("Success", "Credit added successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}