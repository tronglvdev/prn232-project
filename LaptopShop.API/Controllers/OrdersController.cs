using LaptopShop.BLL.DTOs;
using LaptopShop.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;

namespace LaptopShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> Get()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(long userId)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrderDto orderDto)
    {
        var created = await _orderService.CreateOrderAsync(orderDto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] string status)
    {
        await _orderService.UpdateOrderStatusAsync(id, status);
        return NoContent();
    }

    [HttpPost("{id}/request-return")]
    public async Task<IActionResult> RequestReturn(long id)
    {
        await _orderService.RequestReturnAsync(id);
        return Ok(new { Message = "Return requested successfully" });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/approve-return")]
    public async Task<IActionResult> ApproveReturn(long id)
    {
        await _orderService.ApproveReturnAsync(id);
        return Ok(new { Message = "Return approved and inventory updated" });
    }
}
