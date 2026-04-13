using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models;

[Table("purchase_order_items")]
public class PurchaseOrderItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("po_id")]
    public int? PoId { get; set; }
    [ForeignKey("PoId")]
    public virtual PurchaseOrder? PurchaseOrder { get; set; }
    [Column("sku_id")]
    public int? SkuId { get; set; }
    [ForeignKey("SkuId")]
    public virtual ProductSku? ProductSku { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; }
    [Column("unit_cost")]
    public decimal UnitCost { get; set; }
    [Column("subtotal")]
    public decimal Subtotal { get; set; }
}
