using Microsoft.AspNetCore.Mvc;
using MoneyManagement.API.Models;
using MoneyManagement.API.Services;

namespace MoneyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        return Ok(transactions);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PaginatedResult<Transaction>>> GetTransactionsPaged([FromQuery] PaginationParameters paginationParameters)
    {
        var transactions = await _transactionService.GetTransactionsAsync(paginationParameters);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(int id)
    {
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
    {
        var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);
        return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, createdTransaction);
    }

    [HttpPost("transfer")]
    public async Task<ActionResult<Transaction>> TransferMoney(
        [FromQuery] int sourceJarId,
        [FromQuery] int destinationJarId,
        [FromQuery] decimal amount,
        [FromQuery] string description)
    {
        if (amount <= 0)
            return BadRequest("Amount must be greater than zero");

        if (string.IsNullOrWhiteSpace(description))
            return BadRequest("Description is required");

        try
        {
            var transaction = await _transactionService.TransferMoneyAsync(sourceJarId, destinationJarId, amount, description);
            return Ok(transaction);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("One or both jars not found");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("jar/{jarId}")]
    public async Task<ActionResult<PaginatedResult<Transaction>>> GetTransactionsByJar(int jarId, [FromQuery] PaginationParameters paginationParameters)
    {
        var transactions = await _transactionService.GetTransactionsByJarIdAsync(jarId, paginationParameters);
        return Ok(transactions);
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<PaginatedResult<Transaction>>> GetTransactionsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] PaginationParameters paginationParameters)
    {
        if (startDate > endDate)
            return BadRequest("Start date cannot be later than end date");

        var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate, paginationParameters);
        return Ok(transactions);
    }
} 