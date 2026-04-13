using System.ComponentModel.DataAnnotations;

namespace MyApiProject.Models;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; } = null!;         // 商品名稱

    [Required]
    public string SkuCode { get; set; } = null!;       // SKU

    public int? CategoryId { get; set; }               // 商品分類

    [Required]
    [Range(0, double.MaxValue)]
    public decimal RetailPrice { get; set; }           // 售價

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CurrentCost { get; set; }           // 成本

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;        // 當前庫存

    [Range(0, int.MaxValue)]
    public int MinStockLevel { get; set; } = 5;        // 安全庫存

    public bool IsActive { get; set; } = true;         // 商品狀態 (true=上架, false=下架)
}
