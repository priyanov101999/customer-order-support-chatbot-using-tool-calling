import { useEffect, useRef, useState } from "react";
import "./App.css";

import type { ChatMessage } from "./types/chat";
import type { PendingIntent } from "./utils/chatHelpers";

import {
  buildMessageForApi,
  createSessionId,
  formatBotReply,
} from "./utils/chatHelpers";

import { sendChatMessage } from "./api/chatApi";

import ChatHeader from "./components/ChatHeader";
import ChatBox from "./components/ChatBox";
import ChatInput from "./components/ChatInput";

function App() {
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      role: "bot",
      text: "Hi! I am your customer support assistant. Ask me about orders, shipping, returns, refunds, or products.",
    },
  ]);

  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [pendingIntent, setPendingIntent] = useState<PendingIntent>(null);
  const [sessionId, setSessionId] = useState(createSessionId());

  const bottomRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const clearChat = () => {
    setSessionId(createSessionId());
    setPendingIntent(null);

    setMessages([
      {
        role: "bot",
        text: "New chat started. Ask me about orders, shipping, returns, refunds, or products.",
      },
    ]);
  };

  const sendMessage = async () => {
    if (!input.trim() || loading) return;

    const userMessage: ChatMessage = {
      role: "user",
      text: input.trim(),
    };

    const { finalMessage, nextPendingIntent } = buildMessageForApi(
      userMessage.text,
      messages,
      pendingIntent
    );

    console.log("FINAL MESSAGE SENT TO BACKEND:", finalMessage);

    setPendingIntent(nextPendingIntent);
    setMessages((prev) => [...prev, userMessage]);
    setInput("");
    setLoading(true);

    try {
      const data = await sendChatMessage(sessionId, finalMessage);

      console.log("FULL BACKEND RESPONSE:", data);
      console.log("TOOL USED:", data.toolUsed);
      console.log("DATA RETURNED:", data.data);

      const botMessage: ChatMessage = {
        role: "bot",
        text: formatBotReply(data),
        files: data.files ?? [],
      };

      setMessages((prev) => [...prev, botMessage]);
    } catch (error) {
      console.error("Frontend/backend error:", error);

      setMessages((prev) => [
        ...prev,
        {
          role: "bot",
          text: "Sorry, I could not reach the backend server. Please check if the API is running.",
        },
      ]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page">
      <div className="chat-container">
        <ChatHeader sessionId={sessionId} clearChat={clearChat} />

        <ChatBox
          messages={messages}
          loading={loading}
          bottomRef={bottomRef}
        />

        <ChatInput
          input={input}
          setInput={setInput}
          sendMessage={sendMessage}
          loading={loading}
        />
      </div>
    </div>
  );
}

export default App;