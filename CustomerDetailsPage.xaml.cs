using Microsoft.Extensions.DependencyInjection;
using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Views;

public partial class CustomerDetailsPage : ContentPage
{
    private readonly Customer currentCustomer;

    public CustomerDetailsPage(Customer customer)
    {
        InitializeComponent();
        currentCustomer = customer;

        lblCustomerName.Text = customer.FullName;
        lblPhone.Text = customer.PhoneNumber;
        lblCode.Text = customer.CustomerCode;
    }

    private async void OnAddCreditClicked(object sender, EventArgs e)
    {
        var page = Application.Current?.Handler?.MauiContext?.Services.GetService<AddUtangPage>();
        if (page != null)
        {
            page.SetCustomer(currentCustomer);
            await Navigation.PushAsync(page);
        }
    }

    private async void OnRecordPaymentClicked(object sender, EventArgs e)
    {
        var page = Application.Current?.Handler?.MauiContext?.Services.GetService<PaymentsPage>();
        if (page != null)
        {
            await page.SetCustomerAsync(currentCustomer.Id);
            await Navigation.PushAsync(page);
        }
    }

    private async void OnViewStatementClicked(object sender, EventArgs e)
    {
        var page = Application.Current?.Handler?.MauiContext?.Services.GetService<CustomerStatementPage>();
        if (page != null)
        {
            await page.InitializeAsync(currentCustomer.Id, currentCustomer.FullName);
            await Navigation.PushAsync(page);
        }
    }
}