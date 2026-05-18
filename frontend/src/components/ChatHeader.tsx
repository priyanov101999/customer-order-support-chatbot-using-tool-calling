type Props = {
    sessionId: string;
    clearChat: () => void;
};

function ChatHeader({ sessionId, clearChat }: Props) {
    return (
    <div className="chat-header">
    <div>
        <h1>OrderAssist AI</h1>
        <p>
        Ask me anything about orders, shipping, returns, refunds, and products
        </p>
    </div>

    <div className="header-actions">
        <div className="session-id" title={sessionId}>
        #{sessionId.slice(0, 8)}
        </div>

        <button className="clear-button" onClick={clearChat}>
        New Chat
        </button>
    </div>
    </div>
);
}

export default ChatHeader;