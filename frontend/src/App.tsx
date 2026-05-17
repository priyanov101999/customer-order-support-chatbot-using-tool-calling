import { useState } from "react";
import "./App.css";

type ChatMessage = {
  role: "user" | "bot";
  text: string;
};

type ChatResponse = {
  toolUsed?: string;
  data?: unknown;
  message?: string;
};

function App() {
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      role: "bot",
      text: "Hi! I am your customer support assistant. Ask me about orders, shipping, returns, refunds, or products.",
    },
  ]);

  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);

  const sendMessage = async () => {
    if (!input.trim() || loading) return;

    const userMessage: ChatMessage = {
      role: "user",
      text: input.trim(),
    };

    setMessages((prev) => [...prev, userMessage]);
    setInput("");
    setLoading(true);

    try {
      const response = await fetch(
        `${import.meta.env.VITE_API_BASE_URL}/api/chat`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            message: userMessage.text,
          }),
        }
      );

      const data: ChatResponse = await response.json();

      console.log("API status:", response.status);
      console.log("API response:", data);

      const botMessage: ChatMessage = {
        role: "bot",
        text: data.message ?? "No response received.",
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
        <h1>Customer Support Chatbot</h1>

        <div className="chat-box">
          {messages.map((msg, index) => (
            <div
              key={index}
              className={`message-row ${
                msg.role === "user" ? "user-row" : "bot-row"
              }`}
            >
              <div className={`message ${msg.role}`}>{msg.text}</div>
            </div>
          ))}

          {loading && (
            <div className="message-row bot-row">
              <div className="message bot">Thinking...</div>
            </div>
          )}
        </div>

        <div className="input-area">
          <input
              type="text"
              value={input}
              disabled={false}
              onChange={(e) => {
                console.log("Typing:", e.target.value);
                setInput(e.target.value);
              }}
              placeholder="Ask about your order, e.g. Where is order 1?"
              onKeyDown={(e) => {
                if (e.key === "Enter") sendMessage();
              }}
            />

          <button onClick={sendMessage} disabled={loading}>
            Send
          </button>
        </div>
      </div>
    </div>
  );
}

export default App;