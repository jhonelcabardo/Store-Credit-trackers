using SQLite;

namespace Store_Credit_Tracker.Models;

public class AppSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    public bool DarkModeEnabled { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public bool AutoBackupEnabled { get; set; }
}