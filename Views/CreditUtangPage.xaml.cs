using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Views;

public partial class CreditUtangPage : ContentPage
{
    private List<Customer> _customers = new();

    public CreditUtangPage()
    {
        InitializeComponent();
        DueDatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.FullName).ToList();
    }

    private async void OnSaveUtangClicked(object sender, EventArgs e)
    {
        if (CustomerPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Please select a customer.", "OK");
            return;
        }

        if (!decimal.TryParse(AmountEntry.Text, out decimal amount) || amount <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid amount.", "OK");
            return;
        }

        var selectedCustomer = _customers[CustomerPicker.SelectedIndex];

        var transaction = new CreditTransaction
        {
            CustomerId = selectedCustomer.Id,
            ReferenceNumber = $"CR-{DateTime.Now:yyyyMMddHHmmss}",
            Type = TransactionType.CreditAdded,
            Amount = amount,
            TransactionDate = DateTime.Now,
            DueDate = DueDatePicker.Date,
            Notes = DescriptionEditor.Text?.Trim() ?? string.Empty,
            CreatedBy = string.IsNullOrWhiteSpace(App.LoggedInUser) ? "Admin" : App.LoggedInUser,
            IsVoided = false
        };

        await App.Database.AddTransactionAsync(transaction);

        selectedCustomer.TotalDebt += amount;
        await App.Database.UpdateCustomerAsync(selectedCustomer);

        AmountEntry.Text = string.Empty;
        DescriptionEditor.Text = string.Empty;
        CustomerPicker.SelectedIndex = -1;
        DueDatePicker.Date = DateTime.Today;

        await DisplayAlert("Success", "Utang saved successfully.", "OK");
    }
}