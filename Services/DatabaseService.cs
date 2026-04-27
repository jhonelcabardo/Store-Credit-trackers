using SQLite;
using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "storecredittracker.db3"
        );

        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        await _database.CreateTableAsync<Customer>();
        await _database.CreateTableAsync<CreditTransaction>();
        await _database.CreateTableAsync<SinkingFundRecord>();
        await _database.CreateTableAsync<AppSettings>();

        await SeedDefaultSettingsAsync();
    }

    private async Task SeedDefaultSettingsAsync()
    {
        var settings = await _database.Table<AppSettings>()
            .FirstOrDefaultAsync(x => x.Id == 1);

        if (settings == null)
        {
            await _database.InsertAsync(new AppSettings
            {
                Id = 1,
                DarkModeEnabled = false,
                NotificationsEnabled = true,
                AutoBackupEnabled = false
            });
        }
    }

    // =========================
    // CUSTOMER
    // =========================

    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _database.Table<Customer>()
            .Where(x => !x.IsArchived)
            .OrderBy(x => x.FullName)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _database.Table<Customer>()
            .FirstOrDefaultAsync(x => x.Id == customerId && !x.IsArchived);
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _database.InsertAsync(customer);
    }

    public async Task UpdateCustomerAsync(Customer customer)
    {
        await _database.UpdateAsync(customer);
    }

    public async Task<int> GetTotalCustomersAsync()
    {
        return await _database.Table<Customer>()
            .Where(x => !x.IsArchived)
            .CountAsync();
    }

    public async Task<List<string>> GetCustomerNamesAsync()
    {
        var customers = await GetCustomersAsync();

        return customers
            .Select(x => x.FullName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    // =========================
    // CREDIT / UTANG TRANSACTIONS
    // =========================

    public async Task<List<CreditTransaction>> GetAllTransactionsAsync()
    {
        return await _database.Table<CreditTransaction>()
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<CreditTransaction>> GetTransactionsByCustomerIdAsync(int customerId)
    {
        return await _database.Table<CreditTransaction>()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task AddTransactionAsync(CreditTransaction transaction)
    {
        await _database.InsertAsync(transaction);
    }

    public async Task<decimal> GetCustomerBalanceAsync(int customerId)
    {
        var transactions = await _database.Table<CreditTransaction>()
            .Where(t => t.CustomerId == customerId && !t.IsVoided)
            .ToListAsync();

        decimal totalCredit = transactions
            .Where(t => t.Type == TransactionType.CreditAdded ||
                        t.Type == TransactionType.Reversal ||
                        t.Type == TransactionType.Adjustment)
            .Sum(t => t.Amount);

        decimal totalPayment = transactions
            .Where(t => t.Type == TransactionType.Payment)
            .Sum(t => t.Amount);

        return totalCredit - totalPayment;
    }

    public async Task<int> GetTotalCustomersWithBalanceAsync()
    {
        var customers = await GetCustomersAsync();
        int count = 0;

        foreach (var customer in customers)
        {
            var balance = await GetCustomerBalanceAsync(customer.Id);

            if (balance > 0)
                count++;
        }

        return count;
    }

    public async Task<decimal> GetTotalUtangAsync()
    {
        var credits = await _database.Table<CreditTransaction>()
            .Where(t => !t.IsVoided && t.Type == TransactionType.CreditAdded)
            .ToListAsync();

        var payments = await _database.Table<CreditTransaction>()
            .Where(t => !t.IsVoided && t.Type == TransactionType.Payment)
            .ToListAsync();

        return credits.Sum(t => t.Amount) - payments.Sum(t => t.Amount);
    }

    public async Task<decimal> GetTotalPaidAsync()
    {
        var payments = await _database.Table<CreditTransaction>()
            .Where(t => !t.IsVoided && t.Type == TransactionType.Payment)
            .ToListAsync();

        return payments.Sum(t => t.Amount);
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync()
    {
        return await GetRecentTransactionsAsync(10);
    }

    public async Task<List<Transaction>> GetRecentTransactionsAsync(int count)
    {
        var transactions = await _database.Table<CreditTransaction>()
            .Where(t => !t.IsVoided)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var customers = await _database.Table<Customer>()
            .ToListAsync();

        return transactions
            .Take(count)
            .Select(t => new Transaction
            {
                Id = t.Id.ToString(),
                ReferenceNumber = t.ReferenceNumber,
                Type = t.Type.ToString(),
                Amount = t.Amount,
                Date = t.TransactionDate,
                Notes = t.Notes,
                Customer = customers.FirstOrDefault(c => c.Id == t.CustomerId)
            })
            .ToList();
    }

    public async Task<List<Customer>> GetOverdueCustomersAsync()
    {
        var overdueTransactions = await _database.Table<CreditTransaction>()
            .Where(t => !t.IsVoided &&
                        t.Type == TransactionType.CreditAdded &&
                        t.DueDate != null)
            .ToListAsync();

        var overdueIds = overdueTransactions
            .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.Today)
            .Select(t => t.CustomerId)
            .Distinct()
            .ToList();

        var customers = await _database.Table<Customer>()
            .Where(c => !c.IsArchived)
            .ToListAsync();

        return customers
            .Where(c => overdueIds.Contains(c.Id))
            .OrderBy(c => c.FullName)
            .ToList();
    }

    // =========================
    // SINKING FUND
    // =========================

    public async Task AddSinkingFundRecordAsync(SinkingFundRecord record)
    {
        await _database.InsertAsync(record);
    }

    public async Task<List<SinkingFundRecord>> GetSinkingFundRecordsAsync()
    {
        return await _database.Table<SinkingFundRecord>()
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync();
    }

    public async Task<List<SinkingFundRecord>> GetSinkingFundRecordsByMemberAsync(string memberName)
    {
        if (string.IsNullOrWhiteSpace(memberName))
            return new List<SinkingFundRecord>();

        string normalized = memberName.Trim().ToLower();

        var records = await _database.Table<SinkingFundRecord>()
            .ToListAsync();

        return records
            .Where(x => x.MemberName.Trim().ToLower() == normalized)
            .OrderByDescending(x => x.PaymentDate)
            .ToList();
    }

    public async Task<List<string>> GetSinkingFundMemberNamesAsync()
    {
        var records = await _database.Table<SinkingFundRecord>()
            .ToListAsync();

        return records
            .Select(x => x.MemberName?.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList()!;
    }

    public async Task<int> GetTotalSinkingFundMembersAsync()
    {
        var memberNames = await GetSinkingFundMemberNamesAsync();
        return memberNames.Count;
    }

    public async Task<decimal> GetThisSaturdayCollectionAsync()
    {
        DateTime today = DateTime.Today;
        int diff = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
        DateTime thisSaturday = today.AddDays(diff);

        var records = await _database.Table<SinkingFundRecord>()
            .ToListAsync();

        return records
            .Where(x => x.PaymentDate.Date == thisSaturday.Date && x.Status == "Paid")
            .Sum(x => x.Amount);
    }

    public async Task<decimal> GetTotalSinkingFundPaidAsync()
    {
        var records = await _database.Table<SinkingFundRecord>()
            .ToListAsync();

        return records
            .Where(x => x.Status == "Paid")
            .Sum(x => x.Amount);
    }

    public async Task<decimal> GetTotalSinkingFundUnpaidAsync()
    {
        var records = await _database.Table<SinkingFundRecord>()
            .ToListAsync();

        return records
            .Where(x => x.Status == "Unpaid")
            .Sum(x => x.Amount);
    }

    // =========================
    // SETTINGS
    // =========================

    public async Task<AppSettings> GetSettingsAsync()
    {
        var settings = await _database.Table<AppSettings>()
            .FirstOrDefaultAsync(x => x.Id == 1);

        if (settings == null)
        {
            settings = new AppSettings
            {
                Id = 1,
                DarkModeEnabled = false,
                NotificationsEnabled = true,
                AutoBackupEnabled = false
            };

            await _database.InsertAsync(settings);
        }

        return settings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        settings.Id = 1;
        await _database.InsertOrReplaceAsync(settings);
    }

    public async Task UpdateDarkModeAsync(bool isEnabled)
    {
        var settings = await GetSettingsAsync();
        settings.DarkModeEnabled = isEnabled;

        await SaveSettingsAsync(settings);
    }

    public async Task UpdateNotificationsAsync(bool isEnabled)
    {
        var settings = await GetSettingsAsync();
        settings.NotificationsEnabled = isEnabled;

        await SaveSettingsAsync(settings);
    }

    public async Task UpdateAutoBackupAsync(bool isEnabled)
    {
        var settings = await GetSettingsAsync();
        settings.AutoBackupEnabled = isEnabled;

        await SaveSettingsAsync(settings);
    }

    // =========================
    // AUTH PLACEHOLDER
    // =========================

    public Task<bool> RegisterUserAsync(string username, string password)
    {
        return Task.FromResult(true);
    }

    public Task<bool> RegisterUserAsync(string username, string password, string role)
    {
        return Task.FromResult(true);
    }
}