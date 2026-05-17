using CustomerSupport.Api.DTOs;
using CustomerSupport.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CustomerSupport.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ICustomerSupportRepository _repository;

    public ChatController(ICustomerSupportRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> Chat([FromBody] ChatRequestDto request)
    {
        var message = request.Message.ToLower();

        var numberMatch = Regex.Match(message, @"\d+");
        var id = numberMatch.Success ? int.Parse(numberMatch.Value) : 0;

        if (message.Contains("pending refunds"))
        {
            var data = await _repository.GetPendingRefundsAsync();
            return Ok(new ChatResponseDto { ToolUsed = "get_pending_refunds", Data = data, Message = "Here are pending refunds." });
        }

        if (id == 0)
        {
            return BadRequest(new ChatResponseDto { ToolUsed = "none", Message = "Please provide an id." });
        }

        if (message.Contains("where") && message.Contains("order") || message.Contains("summary") && message.Contains("order"))
        {
            var data = await _repository.GetOrderSummaryAsync(id);
            return Ok(new ChatResponseDto { ToolUsed = "get_order_summary", Data = data, Message = "Here is the order summary." });
        }

        if (message.Contains("payment"))
        {
            var data = await _repository.GetPaymentStatusAsync(id);
            return Ok(new ChatResponseDto { ToolUsed = "get_payment_status", Data = data, Message = "Here is the payment status." });
        }

        if (message.Contains("shipment") || message.Contains("tracking") || message.Contains("delivery"))
        {
            var data = await _repository.GetShipmentStatusAsync(id);
            return Ok(new ChatResponseDto { ToolUsed = "get_shipment_status", Data = data, Message = "Here is the shipment status." });
        }

        if (message.Contains("refund") || message.Contains("return"))
        {
            var data = await _repository.GetReturnStatusAsync(id);
            return Ok(new ChatResponseDto { ToolUsed = "get_return_status", Data = data, Message = "Here is the return/refund status." });
        }

        if (message.Contains("customer") && message.Contains("overview"))
        {
            var data = await _repository.GetCustomerOverviewAsync(id);
            return Ok(new ChatResponseDto { ToolUsed = "get_customer_overview", Data = data, Message = "Here is the customer overview." });
        }

        if (message.Contains("orders") && message.Contains("customer"))
        {
            var data = await _repository.GetCustomerOrderHistoryAsync(id);
            return Ok(new ChatResponseDto { ToolUsed = "get_customer_order_history", Data = data, Message = "Here are customer orders." });
        }

        return Ok(new ChatResponseDto
        {
            ToolUsed = "none",
            Message = "I could not understand which tool to call."
        });
    }
}