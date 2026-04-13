using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models;

[Table("product_skus")]
public class ProductSku
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("product_id")]
    public int? ProductId { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
    [Column("sku_code")]
    public string SkuCode { get; set; } = null!;
    [Column("specification")]
    public string? Specification { get; set; }
    [Column("current_cost")]
    public decimal CurrentCost { get; set; }
    [Column("retail_price")]
    public decimal RetailPrice { get; set; }
    [Column("stock_quantity")]
    public int StockQuantity { get; set; } = 0;
    [Column("min_stock_level")]
    public int MinStockLevel { get; set; } = 5;
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
