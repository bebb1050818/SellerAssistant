using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;

namespace MyApiProject.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly PostgresContext _context;

    public DashboardController(PostgresContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<object>> GetDashboard()
    {
        var utcNow    = DateTime.UtcNow;
        var todayUtc  = utcNow.Date;
        var yesterdayUtc = todayUtc.AddDays(-1);
        var monthStart   = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearStart    = new DateTime(utcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // ── Stats 今日 / 昨日 ───────────────────────────────────
        var todayOrders     = await _context.Orders.Where(o => o.CreatedAt >= todayUtc).ToListAsync();
        var yesterdayOrders = await _context.Orders.Where(o => o.CreatedAt >= yesterdayUtc && o.CreatedAt < todayUtc).ToListAsync();

        var todayOrderCount     = todayOrders.Count;
        var yesterdayOrderCount = yesterdayOrders.Count;

        var todayShipCount     = todayOrders.Count(o => o.ShippingStatus != null &&
            (o.ShippingStatus.Contains("ship") || o.ShippingStatus.Contains("出貨")));
        var yesterdayShipCount = yesterdayOrders.Count(o => o.ShippingStatus != null &&
            (o.ShippingStatus.Contains("ship") || o.ShippingStatus.Contains("出貨")));

        var todayRevenue     = todayOrders.Sum(o => o.TotalPrice);
        var yesterdayRevenue = yesterdayOrders.Sum(o => o.TotalPrice);

        // ── 本月 / 上月 營收 ────────────────────────────────────
        var lastMonthStart = monthStart.AddMonths(-1);
        var monthRevenue     = await _context.Orders.Where(o => o.CreatedAt >= monthStart).SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
        var lastMonthRevenue = await _context.Orders.Where(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < monthStart).SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

        // ── 庫存警示數量 ────────────────────────────────────────
        var lowStockCount = await _context.ProductSkus
            .CountAsync(s => s.StockQuantity <= s.MinStockLevel);

        // ── 年度營收趨勢（每月）───────────────────────────────────
        var yearOrders = await _context.Orders
            .Where(o => o.CreatedAt >= yearStart)
            .ToListAsync();

        var revenueTrend = yearOrders
            .GroupBy(o => o.CreatedAt.Month)
            .Select(g => new
            {
                month  = $"{g.Key}月",
                monthNum = g.Key,
                營收   = g.Sum(o => o.TotalPrice)
            })
            .OrderBy(x => x.monthNum)
            .Select(x => new { x.month, x.營收 })
            .ToList();

        // ── 熱銷商品（依本年度訂單項目加總）────────────────────────
        var orderItems = await _context.OrderItems
            .Include(i => i.ProductSku)
            .ThenInclude(s => s!.Product)
            .ToListAsync();

        var topProducts = orderItems
            .Where(i => i.ProductSku?.Product != null)
            .GroupBy(i => new { i.ProductSku!.SkuCode, Name = i.ProductSku.Product!.Name })
            .Select(g => new
            {
                name    = g.Key.Name,
                sku     = g.Key.SkuCode,
                sales   = g.Sum(i => i.Quantity),
                revenue = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(x => x.revenue)
            .Take(5)
            .ToList()
            .Select((p, i) => new { rank = i + 1, p.name, p.sku, p.sales, p.revenue })
            .ToList();

        // ── 庫存警示清單 ────────────────────────────────────────
        var lowStockItems = await _context.ProductSkus
            .Include(s => s.Product)
            .Where(s => s.StockQuantity <= s.MinStockLevel)
            .OrderBy(s => s.StockQuantity)
            .Take(5)
            .Select(s => new
            {
                name  = s.Product!.Name,
                sku   = s.SkuCode,
                stock = s.StockQuantity,
                safe  = s.MinStockLevel
            })
            .ToListAsync();

        // ── 最近訂單 ────────────────────────────────────────────
        var recentOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(6)
            .Select(o => new
            {
                id             = o.OrderNumber,
                amount         = o.TotalPrice,
                paymentStatus  = o.PaymentStatus ?? "",
                shippingStatus = o.ShippingStatus ?? "",
                createdAt      = o.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            stats = new
            {
                todayOrderCount,
                todayOrderChange    = todayOrderCount - yesterdayOrderCount,
                todayShipCount,
                todayShipChange     = todayShipCount - yesterdayShipCount,
                todayRevenue,
                todayRevenueChange  = yesterdayRevenue > 0
                    ? Math.Round((todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100, 1)
                    : (decimal?)null,
                monthRevenue,
                monthRevenueChange  = lastMonthRevenue > 0
                    ? Math.Round((monthRevenue - lastMonthRevenue) / lastMonthRevenue * 100, 1)
                    : (decimal?)null,
                lowStockCount
            },
            revenueTrend,
            topProducts,
            lowStockItems,
            recentOrders
        });
    }
}
