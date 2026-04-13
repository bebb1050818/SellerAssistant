using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Models;

namespace MyApiProject.Services;

public interface IInventoryService
{
    Task<int> GetStockAsync(int skuId);
    Task AddStockAsync(int skuId, int quantity, string reason);
    Task RemoveStockAsync(int skuId, int quantity, string reason);
}

public class InventoryService : IInventoryService
{
    private readonly PostgresContext _context;

    public InventoryService(PostgresContext context) => _context = context;

    public async Task<int> GetStockAsync(int skuId)
    {
        var sku = await _context.ProductSkus.FindAsync(skuId);
        return sku?.StockQuantity ?? 0;
    }

    public async Task AddStockAsync(int skuId, int quantity, string reason)
    {
        var sku = await _context.ProductSkus.FindAsync(skuId) ?? throw new Exception("SKU not found");
        sku.StockQuantity += quantity;
        await _context.SaveChangesAsync();
    }

    public async Task RemoveStockAsync(int skuId, int quantity, string reason)
    {
        var sku = await _context.ProductSkus.FindAsync(skuId) ?? throw new Exception("SKU not found");
        if (sku.StockQuantity < quantity) throw new Exception("Insufficient stock");
        sku.StockQuantity -= quantity;
        await _context.SaveChangesAsync();
    }
}
