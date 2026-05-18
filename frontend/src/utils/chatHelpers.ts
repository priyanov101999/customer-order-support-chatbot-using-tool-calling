import type { ChatMessage, ChatResponse } from "../types/chat";

export type PendingIntent = "order" | "shipping" | "refund" | "return" | null;

export const createSessionId = () => crypto.randomUUID();

export const getLastOrderId = (messages: ChatMessage[]) => {
  const recentMessages = [...messages].reverse();

  for (const msg of recentMessages) {
    const match = msg.text.match(/\b\d+\b/);
    if (match) return match[0];
  }

  return null;
};

export const buildMessageForApi = (
  currentInput: string,
  messages: ChatMessage[],
  pendingIntent: PendingIntent
) => {
  const trimmed = currentInput.trim();
  const lower = trimmed.toLowerCase();

  if (/^\d+$/.test(trimmed)) {
    if (pendingIntent === "refund") {
      return {
        finalMessage: `refund status for order ${trimmed}`,
        nextPendingIntent: null,
      };
    }

    if (pendingIntent === "return") {
      return {
        finalMessage: `return status for order ${trimmed}`,
        nextPendingIntent: null,
      };
    }

    if (pendingIntent === "shipping") {
      return {
        finalMessage: `track shipment for order ${trimmed}`,
        nextPendingIntent: null,
      };
    }

    return {
      finalMessage: `where is my order ${trimmed}`,
      nextPendingIntent: null,
    };
  }

  if (lower.includes("refund")) {
    const orderId = getLastOrderId(messages);

    if (orderId) {
      return {
        finalMessage: `refund status for order ${orderId}`,
        nextPendingIntent: null,
      };
    }

    return {
      finalMessage: trimmed,
      nextPendingIntent: "refund",
    };
  }

  if (lower.includes("return")) {
    const orderId = getLastOrderId(messages);

    if (orderId) {
      return {
        finalMessage: `return status for order ${orderId}`,
        nextPendingIntent: null,
      };
    }

    return {
      finalMessage: trimmed,
      nextPendingIntent: "return",
    };
  }

  if (
    lower.includes("shipping") ||
    lower.includes("shipment") ||
    lower.includes("track") ||
    lower.includes("order")
  ) {
    const orderId = getLastOrderId(messages);

    if (orderId) {
      return {
        finalMessage: `track shipment for order ${orderId}`,
        nextPendingIntent: null,
      };
    }

    return {
      finalMessage: trimmed,
      nextPendingIntent: "shipping",
    };
  }

  return {
    finalMessage: trimmed,
    nextPendingIntent: null,
  };
};

export const formatBotReply = (data: ChatResponse) => {
  if (data.toolUsed === "get_shipment_status" && data.data) {
    const shipmentData = Array.isArray(data.data)
      ? data.data[0]?.data
      : data.data;

    const orderId =
      shipmentData?.orderId ??
      shipmentData?.orderID ??
      shipmentData?.order_id ??
      shipmentData?.id ??
      "your order";

    const status =
      shipmentData?.status ??
      shipmentData?.shipmentStatus ??
      shipmentData?.shippingStatus ??
      shipmentData?.deliveryStatus ??
      "currently being processed";

    const trackingNumber =
      shipmentData?.trackingNumber ??
      shipmentData?.trackingNo ??
      shipmentData?.trackingId ??
      shipmentData?.trackingID ??
      shipmentData?.tracking_number;

    const trackingLink = trackingNumber
      ? `https://www.ups.com/track?tracknum=${trackingNumber}`
      : "";

    return `I found your order ${orderId}. It looks like your shipment is currently ${status.toLowerCase()}.

${
  trackingNumber
    ? `You can track it here: [Track your package](${trackingLink})`
    : `I don't see a tracking number available yet.`
}`;
  }

  if (data.toolUsed === "get_refund_status" && data.data) {
    return `I checked your refund for order ${
      data.data.orderId ?? ""
    }. The refund is currently ${
      data.data.status ?? data.data.refundStatus ?? "being processed"
    }.

The refund amount is ${
      data.data.amount ?? data.data.refundAmount ?? "not available yet"
    }.`;
  }

  if (data.toolUsed === "get_return_status" && data.data) {
    return `I checked your return for order ${
      data.data.orderId ?? ""
    }. The return is currently ${
      data.data.status ?? data.data.returnStatus ?? "being processed"
    }.

Reason: ${data.data.reason ?? "No reason listed."}`;
  }

  return data.message ?? "I could not find a response for that yet.";
};