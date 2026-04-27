using System.Collections.ObjectModel;
using Store_Credit_Tracker.Models;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.ViewModels;

public class CustomerStatementViewModel : BaseViewModel
{
    private readonly LedgerService _ledgerService;
    private string _customerName = string.Empty;
    private decimal _currentBalance;

    public int CustomerId { get; set; }

    public string CustomerName
    {
        get => _customerName;
        set => SetProperty(ref _customerName, value);
    }

    public decimal CurrentBalance
    {
        get => _currentBalance;
        set => SetProperty(ref _currentBalance, value);
    }

    public ObservableCollection<CustomerLedgerItem> Items { get; } = new();

    public CustomerStatementViewModel(LedgerService ledgerService)
    {
        _ledgerService = ledgerService;
    }

    public async Task LoadAsync(int customerId, string customerName)
    {
        CustomerId = customerId;
        CustomerName = customerName;

        Items.Clear();
        var ledger = await _ledgerService.GetCustomerStatementAsync(customerId);
        foreach (var item in ledger)
            Items.Add(item);

        CurrentBalance = await _ledgerService.GetCustomerBalanceAsync(customerId);
    }
}