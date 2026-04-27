using System.Windows.Input;
using Store_Credit_Tracker.Models;
using Store_Credit_Tracker.Services;

namespace Store_Credit_Tracker.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly LedgerService _ledgerService;
    private DashboardSummary _summary = new();

    public DashboardSummary Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    public ICommand LoadCommand { get; }

    public DashboardViewModel(LedgerService ledgerService)
    {
        _ledgerService = ledgerService;
        LoadCommand = new Command(async () => await LoadAsync());
    }

    public async Task LoadAsync()
    {
        Summary = await _ledgerService.GetDashboardSummaryAsync();
    }
}