using Microsoft.Extensions.DependencyInjection;
using Store_Credit_Tracker.ViewModels;


namespace Store_Credit_Tracker.Views;

public partial class CustomerListPage : ContentPage
{
    private readonly CustomerListViewModel _viewModel;

    public CustomerListPage(CustomerListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnApplyFiltersClicked(object sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }
}