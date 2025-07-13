using System.ComponentModel.DataAnnotations;

namespace MoneyManagement.API.Models;

public class Jar
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public decimal Percentage { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public decimal CurrentBalance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
} 