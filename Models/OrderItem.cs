using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models;

[Table("order_items")]
public class OrderItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("order_id")]
    public int? OrderId { get; set; }
    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }
    [Column("sku_id")]
    public int? SkuId { get; set; }
    [ForeignKey("SkuId")]
    public virtual ProductSku? ProductSku { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; }
    [Column("sale_price")]
    public decimal SalePrice { get; set; }
    [Column("cost_at_sale")]
    public decimal CostAtSale { get; set; }
    [Column("subtotal")]
    public decimal Subtotal { get; set; }
}
