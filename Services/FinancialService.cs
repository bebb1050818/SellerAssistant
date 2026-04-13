using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;

namespace MyApiProject.Services;

public interface IFinancialService
{
    Task<object> GetFinancialReportAsync();
}

public class FinancialService : IFinancialService
{
    private readonly PostgresContext _context;

    public FinancialService(PostgresContext context) => _context = context;

    public async Task<object> GetFinancialReportAsync()
    {
        var items = await _context.OrderItems.ToListAsync();
        
        decimal revenue = items.Sum(i => i.SalePrice * i.Quantity);
        decimal cogs = items.Sum(i => i.CostAtSale * i.Quantity);
        decimal grossProfit = revenue - cogs;
        decimal grossMargin = revenue > 0 ? (grossProfit / revenue) * 100 : 0;
        
        return new
        {
            Revenue = revenue,
            COGS = cogs,
            GrossProfit = grossProfit,
            GrossMarginPercentage = grossMargin
        };
    }
}
