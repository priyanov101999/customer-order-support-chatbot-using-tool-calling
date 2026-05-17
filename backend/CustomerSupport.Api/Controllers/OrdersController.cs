using CustomerSupport.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupport.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ICustomerSupportRepository _repository;

    public OrdersController(ICustomerSupportRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{orderId:int}/summary")]
    public async Task<IActionResult> GetOrderSummary(int orderId)
    {
        var result = await _repository.GetOrderSummaryAsync(orderId);
        return result == null ? NotFound(new { message = "Order not found." }) : Ok(result);
    }

    [HttpGet("{orderId:int}/payment")]
    public async Task<IActionResult> GetPaymentStatus(int orderId)
    {
        var result = await _repository.GetPaymentStatusAsync(orderId);
        return result == null ? NotFound(new { message = "Payment not found." }) : Ok(result);
    }

    [HttpGet("{orderId:int}/shipment")]
    public async Task<IActionResult> GetShipmentStatus(int orderId)
    {
        var result = await _repository.GetShipmentStatusAsync(orderId);
        return result == null ? NotFound(new { message = "Shipment not found." }) : Ok(result);
    }

    [HttpGet("{orderId:int}/return")]
    public async Task<IActionResult> GetReturnStatus(int orderId)
    {
        var result = await _repository.GetReturnStatusAsync(orderId);
        return result == null ? NotFound(new { message = "Return not found." }) : Ok(result);
    }
}