using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Core.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AverageCostPrice { get; set; }

    [MaxLength(100)]
    public string? Barcode { get; set; }

    public int StockQuantity { get; set; }

    public int CategoryId { get; set; }
    
    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }

    // Dùng cho Optimistic Concurrency của EF Core
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
