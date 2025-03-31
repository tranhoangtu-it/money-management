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
        try
        {
            var transaction = await _transactionService.TransferMoneyAsync(sourceJarId, destinationJarId, amount, description);
            return Ok(transaction);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("One or both jars not found");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("jar/{jarId}")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByJar(int jarId)
    {
        var transactions = await _transactionService.GetTransactionsByJarIdAsync(jarId);
        return Ok(transactions);
    }

    [HttpGet("daterange")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate);
        return Ok(transactions);
    }
} 