using System.ComponentModel.DataAnnotations;

namespace POS.Core.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
