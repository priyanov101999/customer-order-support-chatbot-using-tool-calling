using CustomerSupport.Api.Orchestration;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.Json.Serialization;
using CustomerSupport.Api.Converters;
namespace CustomerSupport.Api.Services;

public class OpenAiChatOrchestrator : ILlmChatOrchestrator
{
    private readonly ChatClient _chatClient;

    public OpenAiChatOrchestrator()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("OPENAI_API_KEY missing in .env");

        _chatClient = new ChatClient(model, apiKey);
    }
   public async Task<string> GenerateFinalResponseAsync(string userMessage, object toolResults)
{
    var systemPrompt = """
    You are a helpful enterprise customer support assistant.

    Explain the tool results in simple natural language.

    Rules:
    - Do not say "JSON" or "tool result".
    - Do not expose internal tool names.
    - Be concise but helpful.
    - Mention important statuses clearly.
    - If there is a problem, explain it clearly.
    - If data has conflict, point it out.
    - Do not invent information.
    """;

    var toolDataJson = JsonSerializer.Serialize(toolResults);

    var userPrompt = $"""
    User asked:
    {userMessage}

    Data found:
    {toolDataJson}

    Write a helpful customer support response.
    """;

    var completion = await _chatClient.CompleteChatAsync(
    [
        new SystemChatMessage(systemPrompt),
        new UserChatMessage(userPrompt)
    ]);

    return completion.Value.Content[0].Text;
}
    


    public async Task<LlmToolDecision> DecideAsync(string userMessage, List<string> history)
    {
        var conversationContext = string.Join("\n", history);

var userPrompt = $"""
Conversation so far:
{conversationContext}

Latest user message:
{userMessage}
""";
         var systemPrompt = """
You are an enterprise customer support LLM orchestrator.

Your job:
1. Understand the user's message.
2. Decide what information is missing.
3. Ask a follow-up question if required.
4. Choose one or more backend tools.
5. Return ONLY valid JSON.

Available tools:

1. get_order_summary(orderId)
Use for order details, order total, order status, order date, payment summary, shipment summary.
Requires: orderId.

2. get_payment_status(orderId)
Use for payment, paid, charged, billing, failed payment, double charge, transaction, payment gateway.
Requires: orderId.

3. get_shipment_status(orderId)
Use for package, shipment, delivery, tracking, carrier, delayed delivery, not arrived.
Requires: orderId.

4. get_return_status(orderId)
Use for return, refund, money back, refund delay, refund approved, returned item.
Requires: orderId.

5. get_customer_overview(customerId)
Use for customer details, profile, account status, loyalty points, lifetime spend.
Requires: customerId.

6. get_customer_order_history(customerId)
Use for customer purchases, previous orders, order history.
Requires: customerId.

7. get_pending_refunds()
Use for all pending refunds, refund queue, admin refund report.
Requires: no input.

8. search_products(searchText)
Use for product search, product category, brand, stock/product lookup.
Requires: searchText.

9. get_all_orders()
Use for admin/order analytics, all orders, delayed orders, unpaid orders, high-value orders.
Requires: no input.

10. get_all_payments()
Use for payment analytics, failed payments, payment issues, double charge analysis.
Requires: no input.

11. get_all_shipments()
Use for shipment analytics, delayed shipments, carrier issues, delivery problems.
Requires: no input.

12. get_all_returns()
Use for return/refund analytics, pending refunds, return trends.
Requires: no input.

13. get_all_customers()
Use for customer analytics, top customers, account status, loyalty analysis.
Requires: no input.

14. get_all_products()
Use for product analytics, top products, product stock, product catalog.
Requires: no input.

Rules:
- Never invent orderId or customerId.
- If orderId is needed but missing, ask for orderId.
- If customerId is needed but missing, ask for customerId.
- If product search text is needed but missing, ask what product/category/brand to search.
- If user asks a vague issue, ask one useful follow-up question.
- If user describes multiple issues, call multiple tools.
- If user asks analytics, use get_all_* tools and let backend/LLM summarize.
- If request is outside available tools, return unsupported.
- Do not write SQL.
- Do not directly access database.
- Only choose from allowed tools.

JSON format:
{
  "action": "ask_user" | "call_tools" | "unsupported",
  "followUpQuestion": "",
  "tools": [
    {
      "toolName": "",
      "orderId": null,
      "customerId": null,
      "searchText": null
    }
  ]
}
""";

        var completion = await _chatClient.CompleteChatAsync(
        [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        ]);

        var json = completion.Value.Content[0].Text;

        var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
options.Converters.Add(new NullableInt32Converter());

return JsonSerializer.Deserialize<LlmToolDecision>(json, options)
    ?? new LlmToolDecision
    {
        Action = "unsupported",
        FollowUpQuestion = "I could not understand the request."
    };
    }
    
}
