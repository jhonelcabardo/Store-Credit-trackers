namespace Store_Credit_Tracker.Models;

public class DashboardSummary
{
    public decimal TotalOutstanding { get; set; }
    public decimal TotalCollectedToday { get; set; }
    public int CustomersWithBalance { get; set; }
    public int OverdueAccounts { get; set; }
    public int DueTodayCount { get; set; }
}