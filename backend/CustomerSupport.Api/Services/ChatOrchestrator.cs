using CustomerSupport.Api.Orchestration;
using System.Text.RegularExpressions;

namespace CustomerSupport.Api.Services;

public class ChatOrchestrator : IChatOrchestrator
{
    public ChatOrchestrationResult Analyze(string message)
    {
        message = message.ToLower();

        var numberMatch = Regex.Match(message, @"\d+");
        int? id = numberMatch.Success ? int.Parse(numberMatch.Value) : null;

        var intent = DetectIntent(message);

        if (intent == ChatIntent.Unknown)
        {
            return new ChatOrchestrationResult
            {
                Intent = ChatIntent.Unknown,
                NeedsMoreInfo = true,
                FollowUpQuestion = "Can you tell me if this is about an order, payment, shipment, refund, or customer?"
            };
        }

        if (intent == ChatIntent.PendingRefunds)
        {
            return new ChatOrchestrationResult
            {
                Intent = intent,
                NeedsMoreInfo = false
            };
        }

        if (id == null)
        {
            return new ChatOrchestrationResult
            {
                Intent = intent,
                NeedsMoreInfo = true,
                FollowUpQuestion = $"Please provide the ID so I can check the {intent}."
            };
        }

        return new ChatOrchestrationResult
        {
            Intent = intent,
            OrderId = id,
            CustomerId = id,
            NeedsMoreInfo = false
        };
    }

    private static ChatIntent DetectIntent(string message)
    {
        if (message.Contains("pending refund") || message.Contains("refunds pending"))
            return ChatIntent.PendingRefunds;

        if (message.Contains("payment") || message.Contains("paid") || message.Contains("charged") || message.Contains("billing"))
            return ChatIntent.PaymentStatus;

        if (message.Contains("shipment") || message.Contains("shipping") || message.Contains("tracking") || message.Contains("delivery") || message.Contains("package"))
            return ChatIntent.ShipmentStatus;

        if (message.Contains("refund") || message.Contains("return") || message.Contains("money back"))
            return ChatIntent.ReturnStatus;

        if (message.Contains("customer") && (message.Contains("overview") || message.Contains("details") || message.Contains("profile")))
            return ChatIntent.CustomerOverview;

        if (message.Contains("customer") && (message.Contains("orders") || message.Contains("history") || message.Contains("purchases")))
            return ChatIntent.CustomerOrderHistory;

        if (message.Contains("order") || message.Contains("summary") || message.Contains("status"))
            return ChatIntent.OrderSummary;

        return ChatIntent.Unknown;
    }
}