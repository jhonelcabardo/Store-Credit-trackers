namespace Store_Credit_Tracker.Models;

public class Transaction
{
    public string Id { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Customer? Customer { get; set; }
}