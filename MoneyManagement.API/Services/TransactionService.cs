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

    public async Task<PaginatedResult<Transaction>> GetTransactionsAsync(PaginationParameters paginationParameters)
    {
        var totalCount = await _context.Transactions.CountAsync();
        var transactions = await _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((paginationParameters.Page - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        return new PaginatedResult<Transaction>(transactions, paginationParameters.Page, paginationParameters.PageSize, totalCount);
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
        // Validate input
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (sourceJarId == destinationJarId)
            throw new ArgumentException("Source and destination jars cannot be the same");

        // Use database transaction to ensure consistency
        using var dbTransaction = await _context.Database.BeginTransactionAsync();
        try
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

            var result = await CreateTransactionAsync(transaction);
            
            await dbTransaction.CommitAsync();
            return result;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PaginatedResult<Transaction>> GetTransactionsByJarIdAsync(int jarId, PaginationParameters paginationParameters)
    {
        var query = _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .Where(t => t.SourceJarId == jarId || t.DestinationJarId == jarId);

        var totalCount = await query.CountAsync();
        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((paginationParameters.Page - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        return new PaginatedResult<Transaction>(transactions, paginationParameters.Page, paginationParameters.PageSize, totalCount);
    }

    public async Task<PaginatedResult<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, PaginationParameters paginationParameters)
    {
        var query = _context.Transactions
            .Include(t => t.SourceJar)
            .Include(t => t.DestinationJar)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);

        var totalCount = await query.CountAsync();
        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((paginationParameters.Page - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        return new PaginatedResult<Transaction>(transactions, paginationParameters.Page, paginationParameters.PageSize, totalCount);
    }
} 