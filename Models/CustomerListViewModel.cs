using System.Collections.ObjectModel;
using System.Windows.Input;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.ViewModels;

public class CustomerBalanceRow
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int OverdueDays { get; set; }
    public string Status => OverdueDays > 0 ? $"Overdue {OverdueDays}d" : Balance > 0 ? "Active Balance" : "Clear";
}

public class CustomerListViewModel : BaseViewModel
{
    private readonly LedgerService _ledgerService;
    private string _searchText = string.Empty;
    private bool _onlyWithBalance;
    private bool _overdueOnly;

    public ObservableCollection<CustomerBalanceRow> Customers { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool OnlyWithBalance
    {
        get => _onlyWithBalance;
        set => SetProperty(ref _onlyWithBalance, value);
    }

    public bool OverdueOnly
    {
        get => _overdueOnly;
        set => SetProperty(ref _overdueOnly, value);
    }

    public ICommand LoadCommand { get; }

    public CustomerListViewModel(LedgerService ledgerService)
    {
        _ledgerService = ledgerService;
        LoadCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        Customers.Clear();
        var items = await _ledgerService.SearchCustomerBalancesAsync(SearchText, OnlyWithBalance, OverdueOnly);

        foreach (var item in items)
        {
            Customers.Add(new CustomerBalanceRow
			{
				CustomerId = item.Customer.Id,
				CustomerCode = item.Customer.CustomerCode,
				FullName = item.Customer.FullName,
				PhoneNumber = item.Customer.PhoneNumber,
				Balance = item.Balance,
				OverdueDays = item.OverdueDays
			});
		}
    }
}