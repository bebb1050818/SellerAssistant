using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Services;

namespace MyApiProject.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;
    private readonly PostgresContext _context;

    public InventoryController(IInventoryService service, PostgresContext context)
    {
        _service = service;
        _context = context;
    }

    // GET /api/Inventory  → 庫存總覽列表（ProductSku + Product）
    [HttpGet]
    public async Task<ActionResult<object>> GetAll()
    {
        var skus = await _context.ProductSkus
            .Include(s => s.Product)
            .ThenInclude(p => p!.Category)
            .ToListAsync();

        var result = skus.Select(s => new
        {
            id          = s.Id,
            name        = s.Product?.Name ?? "—",
            sku         = s.SkuCode,
            category    = s.Product?.Category?.Name ?? "—",
            current     = s.StockQuantity,
            available   = s.StockQuantity,   // 若有預留邏輯可再調整
            reserved    = 0,
            safe        = s.MinStockLevel,
            status      = s.StockQuantity == 0 ? "缺貨"
                        : s.StockQuantity <= s.MinStockLevel ? "低庫存"
                        : "正常",
            retailPrice = s.RetailPrice,
            currentCost = s.CurrentCost,
            updated     = s.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
        });

        return Ok(result);
    }

    // GET /api/Inventory/{productId}  → 單一 SKU 庫存數量
    [HttpGet("{productId}")]
    public async Task<ActionResult<int>> GetStock(int productId) =>
        Ok(await _service.GetStockAsync(productId));

    // POST /api/Inventory/add  → 增加庫存
    [HttpPost("add")]
    public async Task<IActionResult> AddStock([FromBody] StockRequest request)
    {
        await _service.AddStockAsync(request.SkuId, request.Quantity, request.Reason);
        return Ok();
    }
}

public class StockRequest
{
    public int SkuId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
