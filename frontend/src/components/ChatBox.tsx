import type { ChatMessage } from "../types/chat";
import MessageBubble from "./MessageBubble";

type Props = {
    messages: ChatMessage[];
    loading: boolean;
    bottomRef: React.RefObject<HTMLDivElement>;
};

function ChatBox({ messages, loading, bottomRef }: Props) {
    return (
    <div className="chat-box">
    {messages.map((msg, index) => (
        <MessageBubble key={index} msg={msg} />
    ))}

    {loading && (
        <div className="message-row bot-row">
        <div className="message bot">
            <div className="typing">
            <span></span>
            <span></span>
            <span></span>
            </div>
        </div>
        </div>
    )}

    <div ref={bottomRef} />
    </div>
);
}

export default ChatBox;