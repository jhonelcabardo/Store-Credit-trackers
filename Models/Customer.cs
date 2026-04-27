using SQLite;

namespace Store_Credit_Tracker.Models;

public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public decimal TotalDebt { get; set; }

    [Ignore]
    public List<CreditTransaction> Transactions { get; set; } = new();
}