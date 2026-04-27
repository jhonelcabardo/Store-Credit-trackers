using Store_Credit_Tracker.Models;

namespace Store_Credit_Tracker.Services;

public class LedgerService
{
    private readonly DatabaseService _database;

    public LedgerService(DatabaseService database)
    {
        _database = database;
    }

    public async Task<decimal> GetCustomerBalanceAsync(int customerId)
    {
        decimal balance = 0;

        var transactions = await _database.GetAllTransactionsAsync();

        var customerTransactions = transactions
            .Where(t => t.CustomerId == customerId && !t.IsVoided)
            .OrderBy(t => t.TransactionDate)
            .ToList();

        foreach (var t in customerTransactions)
            balance += GetSignedAmount(t);

        return balance;
    }

    public async Task AddCreditAsync(int customerId, decimal amount, DateTime? dueDate, string notes, string createdBy)
    {
        var transaction = new CreditTransaction
        {
            CustomerId = customerId,
            Amount = amount,
            Type = TransactionType.CreditAdded,
            DueDate = dueDate,
            Notes = notes,
            CreatedBy = createdBy,
            TransactionDate = DateTime.Now,
            ReferenceNumber = $"CR-{DateTime.Now:yyyyMMddHHmmss}",
            IsVoided = false
        };

        await _database.AddTransactionAsync(transaction);

        var customers = await _database.GetCustomersAsync();
        var customer = customers.FirstOrDefault(x => x.Id == customerId);

        if (customer != null)
        {
            customer.TotalDebt += amount;
            await _database.UpdateCustomerAsync(customer);
        }
    }

    public async Task AddPaymentAsync(int customerId, decimal amount, string notes, string createdBy)
    {
        var balance = await GetCustomerBalanceAsync(customerId);

        if (amount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        if (amount > balance)
            throw new InvalidOperationException("Payment cannot be greater than current balance.");

        var transaction = new CreditTransaction
        {
            CustomerId = customerId,
            Amount = amount,
            Type = TransactionType.Payment,
            Notes = notes,
            CreatedBy = createdBy,
            TransactionDate = DateTime.Now,
            ReferenceNumber = $"PMT-{DateTime.Now:yyyyMMddHHmmss}",
            IsVoided = false
        };

        await _database.AddTransactionAsync(transaction);

        var customers = await _database.GetCustomersAsync();
        var customer = customers.FirstOrDefault(x => x.Id == customerId);

        if (customer != null)
        {
            customer.TotalDebt -= amount;
            await _database.UpdateCustomerAsync(customer);
        }
    }

    public async Task<List<CustomerLedgerItem>> GetCustomerStatementAsync(int customerId)
    {
        var transactions = await _database.GetAllTransactionsAsync();

        var customerTransactions = transactions
            .Where(t => t.CustomerId == customerId)
            .OrderBy(t => t.TransactionDate)
            .ToList();

        decimal running = 0;
        var result = new List<CustomerLedgerItem>();

        foreach (var t in customerTransactions)
        {
            if (!t.IsVoided)
                running += GetSignedAmount(t);

            decimal debit = 0;
            decimal credit = 0;

            if (!t.IsVoided)
            {
                var signed = GetSignedAmount(t);
                if (signed > 0) debit = signed;
                if (signed < 0) credit = Math.Abs(signed);
            }

            bool isOverdue = false;
            int overdueDays = 0;

            if (t.Type == TransactionType.CreditAdded &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date < DateTime.Today)
            {
                isOverdue = true;
                overdueDays = (DateTime.Today - t.DueDate.Value.Date).Days;
            }

            result.Add(new CustomerLedgerItem
            {
                TransactionId = t.Id,
                ReferenceNumber = t.ReferenceNumber,
                TransactionDate = t.TransactionDate,
                DueDate = t.DueDate,
                TypeLabel = t.Type.ToString(),
                Debit = debit,
                Credit = credit,
                RunningBalance = running,
                Notes = t.Notes,
                IsOverdue = isOverdue,
                OverdueDays = overdueDays
            });
        }

        return result.OrderByDescending(x => x.TransactionDate).ToList();
    }

    public async Task<List<(Customer Customer, decimal Balance, int OverdueDays)>> SearchCustomerBalancesAsync(
        string? search,
        bool onlyWithBalance = false,
        bool overdueOnly = false)
    {
        var customers = await _database.GetCustomersAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            customers = customers
                .Where(c =>
                    c.FullName.ToLower().Contains(s) ||
                    c.PhoneNumber.ToLower().Contains(s) ||
                    c.CustomerCode.ToLower().Contains(s))
                .ToList();
        }

        var list = new List<(Customer Customer, decimal Balance, int OverdueDays)>();

        foreach (var customer in customers)
        {
            var balance = await GetCustomerBalanceAsync(customer.Id);
            var ledger = await GetCustomerStatementAsync(customer.Id);
            var maxOverdueDays = ledger.Where(x => x.IsOverdue)
                .Select(x => x.OverdueDays)
                .DefaultIfEmpty(0)
                .Max();

            if (onlyWithBalance && balance <= 0)
                continue;

            if (overdueOnly && maxOverdueDays <= 0)
                continue;

            list.Add((customer, balance, maxOverdueDays));
        }

        return list.OrderByDescending(x => x.Balance)
            .ThenBy(x => x.Customer.FullName)
            .ToList();
    }

    public async Task<DashboardSummary> GetDashboardSummaryAsync()
    {
        decimal totalOutstanding = 0;
        decimal totalCollectedToday = 0;
        int customersWithBalance = 0;
        int overdueAccounts = 0;
        int dueTodayCount = 0;

        var customers = await _database.GetCustomersAsync();
        var transactions = await _database.GetAllTransactionsAsync();

        foreach (var customer in customers.Where(c => !c.IsArchived))
        {
            var customerTransactions = transactions
                .Where(t => t.CustomerId == customer.Id && !t.IsVoided)
                .ToList();

            decimal balance = customerTransactions.Sum(GetSignedAmount);

            if (balance > 0)
            {
                totalOutstanding += balance;
                customersWithBalance++;
            }

            bool hasOverdue = customerTransactions.Any(t =>
                t.Type == TransactionType.CreditAdded &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date < DateTime.Today);

            if (hasOverdue)
                overdueAccounts++;

            dueTodayCount += customerTransactions.Count(t =>
                t.Type == TransactionType.CreditAdded &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date == DateTime.Today);

            totalCollectedToday += customerTransactions
                .Where(t => t.Type == TransactionType.Payment &&
                            t.TransactionDate.Date == DateTime.Today)
                .Sum(t => t.Amount);
        }

        return new DashboardSummary
        {
            TotalOutstanding = totalOutstanding,
            TotalCollectedToday = totalCollectedToday,
            CustomersWithBalance = customersWithBalance,
            OverdueAccounts = overdueAccounts,
            DueTodayCount = dueTodayCount
        };
    }

    private static decimal GetSignedAmount(CreditTransaction t)
    {
        if (t.Type == TransactionType.CreditAdded) return t.Amount;
        if (t.Type == TransactionType.Payment) return -t.Amount;
        if (t.Type == TransactionType.Reversal) return t.Amount;
        if (t.Type == TransactionType.Adjustment) return t.Amount;
        return 0m;
    }
}