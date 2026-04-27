using System.Collections.ObjectModel;
using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Views;

public partial class CustomerPage : ContentPage
{
    private ObservableCollection<Customer> _customers = new();
    private bool _isCustomerListVisible = false;

    public CustomerPage()
    {
        InitializeComponent();
        CustomerCollectionView.ItemsSource = _customers;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        var customers = await App.Database.GetCustomersAsync();
        _customers.Clear();

        foreach (var customer in customers)
            _customers.Add(customer);
    }

    private void OnToggleCustomerListClicked(object sender, EventArgs e)
    {
        _isCustomerListVisible = !_isCustomerListVisible;
        CustomerCollectionView.IsVisible = _isCustomerListVisible;
        ToggleCustomerListButton.Text = _isCustomerListVisible ? "Hide" : "Show";
    }

    private async void OnAddCustomerClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text))
        {
            await DisplayAlert("Error", "Please enter customer name.", "OK");
            return;
        }

        var customer = new Customer
        {
            CustomerCode = $"CUST{DateTime.Now:yyyyMMddHHmmss}",
            FullName = CustomerNameEntry.Text.Trim(),
            PhoneNumber = ContactEntry.Text?.Trim() ?? string.Empty,
            IsArchived = false,
            TotalDebt = 0
        };

        await App.Database.AddCustomerAsync(customer);
        await LoadCustomersAsync();

        CustomerNameEntry.Text = string.Empty;
        ContactEntry.Text = string.Empty;
        AddressEditor.Text = string.Empty;

        await DisplayAlert("Success", "Customer added successfully.", "OK");
    }
}