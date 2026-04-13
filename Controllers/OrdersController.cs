using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiProject.Models;
using MyApiProject.Services;

namespace MyApiProject.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll() => Ok(await _service.GetAllOrdersAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var order = await _service.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderRequest request)
    {
        var order = await _service.CreateOrderAsync(request.OrderNumber, request.Items.Select(i => (i.SkuId, i.Quantity)).ToList());
        return Ok(order);
    }
}

public class OrderRequest
{
    public string OrderNumber { get; set; } = string.Empty;
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public int SkuId { get; set; }
    public int Quantity { get; set; }
}
