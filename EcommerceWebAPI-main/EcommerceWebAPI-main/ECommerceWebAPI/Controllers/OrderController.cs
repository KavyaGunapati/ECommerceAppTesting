
using EcommerceWebAPI.Interfaces.IService;
using EcommerceWebAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> PlaceOrder(CreateOrder dto)
    {
        var userId = User.FindFirst("sub")?.Value;
        var result = await _orderService.PlaceOrderAsync(userId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetOrdersByUser(string userId)
    {
        var result = await _orderService.GetOrdersByUserAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _orderService.CancelOrderAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
