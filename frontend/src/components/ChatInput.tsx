type Props = {
    input: string;
    setInput: (value: string) => void;
    sendMessage: () => void;
    loading: boolean;
};

function ChatInput({
    input,
    setInput,
    sendMessage,
    loading,
}: Props) {
    return (
    <div className="input-area">
    <input
        type="text"
        value={input}
        onChange={(e) => setInput(e.target.value)}
        placeholder="Ask: Where is order 1?"
        onKeyDown={(e) => {
        if (e.key === "Enter") sendMessage();
        }}
    />

    <button onClick={sendMessage} disabled={loading}>
        Send
    </button>
    </div>
);
}

export default ChatInput;