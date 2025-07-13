using MoneyManagement.API.Models;

namespace MoneyManagement.API.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<PaginatedResult<Transaction>> GetTransactionsAsync(PaginationParameters paginationParameters);
    Task<Transaction?> GetTransactionByIdAsync(int id);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> TransferMoneyAsync(int sourceJarId, int destinationJarId, decimal amount, string description);
    Task<PaginatedResult<Transaction>> GetTransactionsByJarIdAsync(int jarId, PaginationParameters paginationParameters);
    Task<PaginatedResult<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, PaginationParameters paginationParameters);
} 