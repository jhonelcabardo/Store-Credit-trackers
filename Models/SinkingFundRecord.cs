using SQLite;

namespace Store_Credit_Tracker.Models;

public class SinkingFundRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string MemberName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "Unpaid";
}