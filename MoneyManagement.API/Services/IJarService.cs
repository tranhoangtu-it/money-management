using MoneyManagement.API.Models;

namespace MoneyManagement.API.Services;

public interface IJarService
{
    Task<IEnumerable<Jar>> GetAllJarsAsync();
    Task<Jar?> GetJarByIdAsync(int id);
    Task<Jar> CreateJarAsync(Jar jar);
    Task<Jar> UpdateJarAsync(int id, Jar jar);
    Task<bool> DeleteJarAsync(int id);
    Task<decimal> GetJarBalanceAsync(int id);
    Task<Jar> AddMoneyToJarAsync(int id, decimal amount);
    Task<Jar> RemoveMoneyFromJarAsync(int id, decimal amount);
} 