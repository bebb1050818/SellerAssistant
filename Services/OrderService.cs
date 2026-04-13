using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Models;

namespace MyApiProject.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<object?> GetOrderByIdAsync(int id);
    Task<Order> CreateOrderAsync(string orderNumber, List<(int SkuId, int Quantity)> items);
}

public class OrderService : IOrderService
{
    private readonly PostgresContext _context;
    private readonly IInventoryService _inventory;

    public OrderService(PostgresContext context, IInventoryService inventory)
    {
        _context = context;
        _inventory = inventory;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
        await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();

    public async Task<object?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return null;

        var items = await _context.OrderItems
            .Where(i => i.OrderId == id)
            .Include(i => i.ProductSku)
            .Select(i => new
            {
                i.Id,
                i.SkuId,
                SkuCode = i.ProductSku != null ? i.ProductSku.SkuCode : null,
                i.Quantity,
                i.SalePrice,
                i.CostAtSale,
                i.Subtotal
            })
            .ToListAsync();

        return new
        {
            order.Id,
            order.OrderNumber,
            order.TotalPrice,
            order.PaymentStatus,
            order.ShippingStatus,
            order.CreatedAt,
            Items = items
        };
    }

    public async Task<Order> CreateOrderAsync(string orderNumber, List<(int SkuId, int Quantity)> items)
    {
        var order = new Order { OrderNumber = orderNumber };
        decimal total = 0;
        
        foreach (var item in items)
        {
            var sku = await _context.ProductSkus.FindAsync(item.SkuId) 
                ?? throw new Exception("SKU not found");
                
            await _inventory.RemoveStockAsync(sku.Id, item.Quantity, "Sale");
            
            var orderItem = new OrderItem
            {
                SkuId = sku.Id,
                Quantity = item.Quantity,
                SalePrice = sku.RetailPrice,
                CostAtSale = sku.CurrentCost,
                Subtotal = sku.RetailPrice * item.Quantity
            };
            
            total += orderItem.Subtotal;
            _context.OrderItems.Add(orderItem);
        }

        order.TotalPrice = total;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }
}
