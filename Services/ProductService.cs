using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Models;

namespace MyApiProject.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<object> CreateProductWithSkuAsync(CreateProductRequest request);
    Task<object?> UpdateProductWithSkuAsync(int productId, CreateProductRequest request);
    Task<bool> DeleteProductAsync(int productId);
}

public class ProductService : IProductService
{
    private readonly PostgresContext _context;

    public ProductService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync() =>
        await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductSkus)
            .ToListAsync();

    public async Task<Product?> GetProductByIdAsync(int id) => await _context.Products.FindAsync(id);

    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<object> CreateProductWithSkuAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var sku = new ProductSku
        {
            ProductId = product.Id,
            SkuCode = request.SkuCode,
            RetailPrice = request.RetailPrice,
            CurrentCost = request.CurrentCost,
            StockQuantity = request.StockQuantity,
            MinStockLevel = request.MinStockLevel,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ProductSkus.Add(sku);
        await _context.SaveChangesAsync();

        return new
        {
            product.Id,
            product.Name,
            product.CategoryId,
            product.IsActive,
            product.CreatedAt,
            Sku = new
            {
                sku.Id,
                sku.SkuCode,
                sku.RetailPrice,
                sku.CurrentCost,
                sku.StockQuantity,
                sku.MinStockLevel
            }
        };
    }

    public async Task<object?> UpdateProductWithSkuAsync(int productId, CreateProductRequest request)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return null;

        product.Name = request.Name;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;

        var sku = await _context.ProductSkus.FirstOrDefaultAsync(s => s.ProductId == productId);
        if (sku != null)
        {
            sku.SkuCode = request.SkuCode;
            sku.RetailPrice = request.RetailPrice;
            sku.CurrentCost = request.CurrentCost;
            sku.StockQuantity = request.StockQuantity;
            sku.MinStockLevel = request.MinStockLevel;
            sku.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return new
        {
            product.Id,
            product.Name,
            product.CategoryId,
            product.IsActive,
            product.CreatedAt,
            Sku = sku == null ? null : new
            {
                sku.Id,
                sku.SkuCode,
                sku.RetailPrice,
                sku.CurrentCost,
                sku.StockQuantity,
                sku.MinStockLevel
            }
        };
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
