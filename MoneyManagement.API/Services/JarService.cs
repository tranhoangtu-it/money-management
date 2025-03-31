using Microsoft.EntityFrameworkCore;
using MoneyManagement.API.Data;
using MoneyManagement.API.Models;

namespace MoneyManagement.API.Services;

public class JarService : IJarService
{
    private readonly ApplicationDbContext _context;

    public JarService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Jar>> GetAllJarsAsync()
    {
        return await _context.Jars.ToListAsync();
    }

    public async Task<Jar?> GetJarByIdAsync(int id)
    {
        return await _context.Jars.FindAsync(id);
    }

    public async Task<Jar> CreateJarAsync(Jar jar)
    {
        _context.Jars.Add(jar);
        await _context.SaveChangesAsync();
        return jar;
    }

    public async Task<Jar> UpdateJarAsync(int id, Jar jar)
    {
        var existingJar = await _context.Jars.FindAsync(id);
        if (existingJar == null)
            throw new KeyNotFoundException($"Jar with ID {id} not found");

        existingJar.Name = jar.Name;
        existingJar.Percentage = jar.Percentage;
        existingJar.Description = jar.Description;
        existingJar.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingJar;
    }

    public async Task<bool> DeleteJarAsync(int id)
    {
        var jar = await _context.Jars.FindAsync(id);
        if (jar == null)
            return false;

        _context.Jars.Remove(jar);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetJarBalanceAsync(int id)
    {
        var jar = await _context.Jars.FindAsync(id);
        if (jar == null)
            throw new KeyNotFoundException($"Jar with ID {id} not found");

        return jar.CurrentBalance;
    }

    public async Task<Jar> AddMoneyToJarAsync(int id, decimal amount)
    {
        var jar = await _context.Jars.FindAsync(id);
        if (jar == null)
            throw new KeyNotFoundException($"Jar with ID {id} not found");

        jar.CurrentBalance += amount;
        jar.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return jar;
    }

    public async Task<Jar> RemoveMoneyFromJarAsync(int id, decimal amount)
    {
        var jar = await _context.Jars.FindAsync(id);
        if (jar == null)
            throw new KeyNotFoundException($"Jar with ID {id} not found");

        if (jar.CurrentBalance < amount)
            throw new InvalidOperationException($"Insufficient funds in jar {jar.Name}");

        jar.CurrentBalance -= amount;
        jar.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return jar;
    }
} 