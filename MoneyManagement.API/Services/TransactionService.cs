using Microsoft.EntityFrameworkCore;
using MoneyManagement.API.Data;
using MoneyManagement.API.Models;

namespace MoneyManagement.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IJarService _jarService;

    public TransactionService(ApplicationDbContext context, IJarService jarService)
    {
        _context = context;
        _jarService = jarService;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return await _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> TransferMoneyAsync(int sourceJarId, int destinationJarId, decimal amount, string description)
    {
        // Remove money from source jar
        await _jarService.RemoveMoneyFromJarAsync(sourceJarId, amount);

        // Add money to destination jar
        await _jarService.AddMoneyToJarAsync(destinationJarId, amount);

        var transaction = new Transaction
        {
            SourceJarId = sourceJarId,
            DestinationJarId = destinationJarId,
            Amount = amount,
            Description = description,
            TransactionDate = DateTime.UtcNow
        };

        return await CreateTransactionAsync(transaction);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByJarIdAsync(int jarId)
    {
        return await _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .Where(t => t.SourceJarId == jarId || t.DestinationJarId == jarId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
} 