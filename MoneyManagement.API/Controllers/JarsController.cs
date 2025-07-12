using Microsoft.AspNetCore.Mvc;
using MoneyManagement.API.Models;
using MoneyManagement.API.Services;

namespace MoneyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JarsController : ControllerBase
{
    private readonly IJarService _jarService;

    public JarsController(IJarService jarService)
    {
        _jarService = jarService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Jar>>> GetJars()
    {
        var jars = await _jarService.GetAllJarsAsync();
        return Ok(jars);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PaginatedResult<Jar>>> GetJarsPaged([FromQuery] PaginationParameters paginationParameters)
    {
        var jars = await _jarService.GetJarsAsync(paginationParameters);
        return Ok(jars);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Jar>> GetJar(int id)
    {
        var jar = await _jarService.GetJarByIdAsync(id);
        if (jar == null)
            return NotFound();

        return Ok(jar);
    }

    [HttpPost]
    public async Task<ActionResult<Jar>> CreateJar(Jar jar)
    {
        var createdJar = await _jarService.CreateJarAsync(jar);
        return CreatedAtAction(nameof(GetJar), new { id = createdJar.Id }, createdJar);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Jar>> UpdateJar(int id, Jar jar)
    {
        try
        {
            var updatedJar = await _jarService.UpdateJarAsync(id, jar);
            return Ok(updatedJar);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteJar(int id)
    {
        var result = await _jarService.DeleteJarAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/balance")]
    public async Task<ActionResult<decimal>> GetJarBalance(int id)
    {
        try
        {
            var balance = await _jarService.GetJarBalanceAsync(id);
            return Ok(balance);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/add")]
    public async Task<ActionResult<Jar>> AddMoney(int id, [FromBody] decimal amount)
    {
        if (amount <= 0)
            return BadRequest("Amount must be greater than zero");

        try
        {
            var updatedJar = await _jarService.AddMoneyToJarAsync(id, amount);
            return Ok(updatedJar);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Jar with ID {id} not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("{id}/remove")]
    public async Task<ActionResult<Jar>> RemoveMoney(int id, [FromBody] decimal amount)
    {
        if (amount <= 0)
            return BadRequest("Amount must be greater than zero");

        try
        {
            var updatedJar = await _jarService.RemoveMoneyFromJarAsync(id, amount);
            return Ok(updatedJar);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Jar with ID {id} not found");
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
} 