using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.Views;

public partial class CustomerStatementPage : ContentPage
{
    private readonly LedgerService _ledgerService;

    public CustomerStatementPage(LedgerService ledgerService)
    {
        InitializeComponent();
        _ledgerService = ledgerService;
    }

    public async Task InitializeAsync(int customerId, string customerName)
    {
        lblTitle.Text = $"{customerName} Statement";
        statementList.ItemsSource = await _ledgerService.GetCustomerStatementAsync(customerId);
    }
}