namespace Store_Credit_Tracker.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string EntityName { get; set; } = "";
    public int EntityId { get; set; }
    public string Action { get; set; } = "";
    public string PerformedBy { get; set; } = "";
    public string Details { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}