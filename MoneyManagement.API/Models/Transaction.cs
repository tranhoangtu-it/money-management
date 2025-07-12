using System.ComponentModel.DataAnnotations;

namespace MoneyManagement.API.Models;

public class Transaction
{
    public int Id { get; set; }

    [Required]
    public int SourceJarId { get; set; }

    [Required]
    public int DestinationJarId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Jar? SourceJar { get; set; }
    public Jar? DestinationJar { get; set; }
} 