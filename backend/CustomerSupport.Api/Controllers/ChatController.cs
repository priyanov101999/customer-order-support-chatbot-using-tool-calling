using CustomerSupport.Api.DTOs;
using CustomerSupport.Api.Orchestration;
using CustomerSupport.Api.Repositories;
using CustomerSupport.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupport.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ICustomerSupportRepository _repository;
    private readonly ILlmChatOrchestrator _orchestrator;
    private readonly IChatMemoryService _memory;

    public ChatController(
        ICustomerSupportRepository repository,
        ILlmChatOrchestrator orchestrator,
        IChatMemoryService memory)
    {
        _repository = repository;
        _orchestrator = orchestrator;
        _memory = memory;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> Chat([FromBody] ChatRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new ChatResponseDto
            {
                ToolUsed = "none",
                Message = "Please enter a message."
            });
        }

        var sessionId = string.IsNullOrWhiteSpace(request.SessionId)
            ? "default"
            : request.SessionId;

        var history = _memory.GetHistory(sessionId);

        _memory.AddMessage(sessionId, "user", request.Message);

        var decision = await _orchestrator.DecideAsync(request.Message, history);

        if (decision.Action == "ask_user")
        {
            _memory.AddMessage(sessionId, "assistant", decision.FollowUpQuestion);

            return Ok(new ChatResponseDto
            {
                ToolUsed = "collect_missing_details",
                Message = decision.FollowUpQuestion
            });
        }

        if (decision.Action == "unsupported")
        {
            _memory.AddMessage(sessionId, "assistant", decision.FollowUpQuestion);

            return Ok(new ChatResponseDto
            {
                ToolUsed = "unsupported",
                Message = decision.FollowUpQuestion
            });
        }

        var results = new List<object>();

        foreach (var tool in decision.Tools)
        {
            object? data;

            switch (tool.ToolName)
            {
                case "get_order_summary":
                    if (!TryGetOrderId(tool, out var orderSummaryId))
                        return AskForOrderId(sessionId);

                    data = await _repository.GetOrderSummaryAsync(orderSummaryId);
                    break;

                case "get_payment_status":
                    if (!TryGetOrderId(tool, out var paymentOrderId))
                        return AskForOrderId(sessionId);

                    data = await _repository.GetPaymentStatusAsync(paymentOrderId);
                    break;

                case "get_shipment_status":
                    if (!TryGetOrderId(tool, out var shipmentOrderId))
                        return AskForOrderId(sessionId);

                    data = await _repository.GetShipmentStatusAsync(shipmentOrderId);
                    break;

                case "get_return_status":
                    if (!TryGetOrderId(tool, out var returnOrderId))
                        return AskForOrderId(sessionId);

                    data = await _repository.GetReturnStatusAsync(returnOrderId);
                    break;

                case "get_customer_overview":
                    if (!TryGetCustomerId(tool, out var overviewCustomerId))
                        return AskForCustomerId(sessionId);

                    data = await _repository.GetCustomerOverviewAsync(overviewCustomerId);
                    break;

                case "get_customer_order_history":
                    if (!TryGetCustomerId(tool, out var historyCustomerId))
                        return AskForCustomerId(sessionId);

                    data = await _repository.GetCustomerOrderHistoryAsync(historyCustomerId);
                    break;

                case "get_pending_refunds":
                    data = await _repository.GetPendingRefundsAsync();
                    break;

                case "search_products":
                    if (string.IsNullOrWhiteSpace(tool.SearchText))
                    {
                        var msg = "Please tell me what product, brand, or category you want to search.";
                        _memory.AddMessage(sessionId, "assistant", msg);

                        return Ok(new ChatResponseDto
                        {
                            ToolUsed = "collect_missing_details",
                            Message = msg
                        });
                    }

                    data = await _repository.SearchProductsAsync(tool.SearchText);
                    break;

                case "get_all_customers":
                    data = await _repository.GetAllCustomersAsync();
                    break;

                case "get_all_orders":
                    data = await _repository.GetAllOrdersAsync();
                    break;

                case "get_all_payments":
                    data = await _repository.GetAllPaymentsAsync();
                    break;

                case "get_all_shipments":
                    data = await _repository.GetAllShipmentsAsync();
                    break;

                case "get_all_returns":
                    data = await _repository.GetAllReturnsAsync();
                    break;

                case "get_all_products":
                    data = await _repository.GetAllProductsAsync();
                    break;

                default:
                    var unsupportedMsg = $"The selected tool '{tool.ToolName}' is not supported.";
                    _memory.AddMessage(sessionId, "assistant", unsupportedMsg);

                    return Ok(new ChatResponseDto
                    {
                        ToolUsed = "unsupported",
                        Message = unsupportedMsg
                    });
            }

            results.Add(new
            {
                Tool = tool.ToolName,
                Data = data
            });
        }

        var finalMessage = await _orchestrator.GenerateFinalResponseAsync(
    request.Message,
    results
);

        _memory.AddMessage(sessionId, "assistant", finalMessage);

        return Ok(new ChatResponseDto
        {
            ToolUsed = string.Join(", ", decision.Tools.Select(t => t.ToolName)),
            Data = results,
            Message = finalMessage
        });
    }

    private bool TryGetOrderId(ToolCallRequest tool, out int orderId)
    {
        orderId = 0;

        if (tool.OrderId == null)
            return false;

        return int.TryParse(tool.OrderId.ToString(), out orderId);
    }

    private bool TryGetCustomerId(ToolCallRequest tool, out int customerId)
    {
        customerId = 0;

        if (tool.CustomerId == null)
            return false;

        return int.TryParse(tool.CustomerId.ToString(), out customerId);
    }

    private OkObjectResult AskForOrderId(string sessionId)
    {
        var msg = "Please provide the order ID so I can continue.";
        _memory.AddMessage(sessionId, "assistant", msg);

        return Ok(new ChatResponseDto
        {
            ToolUsed = "collect_missing_details",
            Message = msg
        });
    }

    private OkObjectResult AskForCustomerId(string sessionId)
    {
        var msg = "Please provide the customer ID so I can continue.";
        _memory.AddMessage(sessionId, "assistant", msg);

        return Ok(new ChatResponseDto
        {
            ToolUsed = "collect_missing_details",
            Message = msg
        });
    }
}