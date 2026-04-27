namespace Store_Credit_Tracker.Models;

public class CustomerLedgerItem
{
    public int TransactionId { get; set; }
    public string ReferenceNumber { get; set; } = "";
    public DateTime TransactionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string TypeLabel { get; set; } = "";
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal RunningBalance { get; set; }
    public string Notes { get; set; } = "";
    public bool IsOverdue { get; set; }
    public int OverdueDays { get; set; }
}