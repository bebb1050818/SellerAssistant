using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiProject.Models;
using MyApiProject.Services;
using System.ComponentModel.DataAnnotations;

namespace MyApiProject.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<object>> GetAll()
    {
        var products = await _service.GetAllProductsAsync();
        var result = products.Select(p =>
        {
            var sku = p.ProductSkus.FirstOrDefault();
            return new
            {
                id          = p.Id,
                name        = p.Name,
                categoryId  = p.CategoryId,
                category    = p.Category?.Name ?? "—",
                isActive    = p.IsActive,
                status      = p.IsActive ? "上架" : "下架",
                createdAt   = p.CreatedAt,
                // SKU 資料（取第一筆）
                skuId       = sku?.Id,
                sku         = sku?.SkuCode ?? "—",
                retailPrice = sku?.RetailPrice ?? 0,
                currentCost = sku?.CurrentCost ?? 0,
                stock       = sku?.StockQuantity ?? 0,
                safeStock   = sku?.MinStockLevel ?? 0,
            };
        });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product) => Ok(await _service.CreateProductAsync(product));

    [HttpPost("create-with-sku")]
    public async Task<ActionResult<object>> CreateWithSku([FromBody] CreateProductRequest request)
    {
        var result = await _service.CreateProductWithSkuAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<object>> Update(int id, [FromBody] CreateProductRequest request)
    {
        var result = await _service.UpdateProductWithSkuAsync(id, request);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteProductAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
