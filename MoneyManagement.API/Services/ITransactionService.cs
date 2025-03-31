using MoneyManagement.API.Models;

namespace MoneyManagement.API.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction?> GetTransactionByIdAsync(int id);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> TransferMoneyAsync(int sourceJarId, int destinationJarId, decimal amount, string description);
    Task<IEnumerable<Transaction>> GetTransactionsByJarIdAsync(int jarId);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
} 