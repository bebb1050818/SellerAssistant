using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models;

[Table("orders")]
public class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("order_number")]
    public string OrderNumber { get; set; } = null!;
    [Column("total_price")]
    public decimal TotalPrice { get; set; }
    [Column("payment_status")]
    public string? PaymentStatus { get; set; }
    [Column("shipping_status")]
    public string? ShippingStatus { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
