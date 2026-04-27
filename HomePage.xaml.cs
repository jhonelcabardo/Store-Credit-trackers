namespace Store_Credit_Tracker.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        LoadDashboard();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDashboard();
    }

    private async void LoadDashboard()
    {
        int totalCustomers = await App.Database.GetTotalCustomersAsync();
        decimal totalUtang = await App.Database.GetTotalUtangAsync();
        int totalSinkingFundMembers = await App.Database.GetTotalSinkingFundMembersAsync();
        decimal saturdayCollection = await App.Database.GetThisSaturdayCollectionAsync();

        TotalCustomersLabel.Text = totalCustomers.ToString();
        TotalUtangLabel.Text = $"₱{totalUtang:N2}";
        TotalMembersLabel.Text = totalSinkingFundMembers.ToString();
        SaturdayCollectionLabel.Text = $"₱{saturdayCollection:N2}";
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Home", "You are already on Home.", "OK");
    }

    private async void OnCreditUtangClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreditUtangPage());
    }

    private async void OnSinkingFundClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SinkingFundPage());
    }

    private async void OnCustomerClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CustomerPage());
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }
}