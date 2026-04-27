using SQLite;

namespace Store_Credit_Tracker.Models;

public class CreditTransaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsVoided { get; set; }
}