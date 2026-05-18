import type { ChatResponse } from "../types/chat";

export const sendChatMessage = async (
    sessionId: string,
    message: string
): Promise<ChatResponse> => {
    const response = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/chat`,
    {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
    },
        body: JSON.stringify({
        sessionId,
        message,
    }),
    }
    );

    return response.json();
};