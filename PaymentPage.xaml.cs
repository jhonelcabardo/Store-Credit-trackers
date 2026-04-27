using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Views;

public partial class PaymentsPage : ContentPage
{
    private List<Customer> _customers = new();
    private int? _preselectedCustomerId;

    public PaymentsPage()
    {
        InitializeComponent();
    }

    public async Task SetCustomerAsync(int customerId)
    {
        _preselectedCustomerId = customerId;

        if (_customers.Count == 0)
            await LoadCustomersAsync();

        var index = _customers.FindIndex(c => c.Id == customerId);
        if (index >= 0)
            CustomerPicker.SelectedIndex = index;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCustomersAsync();

        if (_preselectedCustomerId.HasValue)
        {
            var index = _customers.FindIndex(c => c.Id == _preselectedCustomerId.Value);
            if (index >= 0)
                CustomerPicker.SelectedIndex = index;
        }
    }

    private async Task LoadCustomersAsync()
    {
        _customers = await App.Database.GetCustomersAsync();
        CustomerPicker.ItemsSource = _customers.Select(c => c.FullName).ToList();
    }

    private async void OnPayClicked(object sender, EventArgs e)
    {
        if (CustomerPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Error", "Please select a customer.", "OK");
            return;
        }

        if (!decimal.TryParse(AmountEntry.Text, out decimal amount) || amount <= 0)
        {
            await DisplayAlert("Error", "Please enter a valid payment amount.", "OK");
            return;
        }

        var selectedCustomer = _customers[CustomerPicker.SelectedIndex];

        if (amount > selectedCustomer.TotalDebt)
        {
            await DisplayAlert("Error", "Payment cannot be greater than current utang.", "OK");
            return;
        }

        var transaction = new CreditTransaction
        {
            CustomerId = selectedCustomer.Id,
            ReferenceNumber = $"PMT-{DateTime.Now:yyyyMMddHHmmss}",
            Type = TransactionType.Payment,
            Amount = amount,
            TransactionDate = DateTime.Now,
            DueDate = null,
            Notes = NotesEditor.Text?.Trim() ?? string.Empty,
            CreatedBy = string.IsNullOrWhiteSpace(App.LoggedInUser) ? "Admin" : App.LoggedInUser,
            IsVoided = false
        };

        await App.Database.AddTransactionAsync(transaction);

        selectedCustomer.TotalDebt -= amount;
        if (selectedCustomer.TotalDebt < 0)
            selectedCustomer.TotalDebt = 0;

        await App.Database.UpdateCustomerAsync(selectedCustomer);

        AmountEntry.Text = string.Empty;
        NotesEditor.Text = string.Empty;
        CustomerPicker.SelectedIndex = -1;

        await DisplayAlert("Success", "Payment saved successfully.", "OK");
    }
}