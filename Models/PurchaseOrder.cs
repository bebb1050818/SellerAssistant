using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models;

[Table("purchase_orders")]
public class PurchaseOrder
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("supplier_id")]
    public int? SupplierId { get; set; }
    [ForeignKey("SupplierId")]
    public virtual Supplier? Supplier { get; set; }
    [Column("order_date")]
    public DateTime OrderDate { get; set; }
    [Column("total_amount")]
    public decimal TotalAmount { get; set; }
    [Column("status")]
    public string Status { get; set; } = "pending";
    [Column("remark")]
    public string? Remark { get; set; }
}
