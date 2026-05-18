import ReactMarkdown from "react-markdown";
import type { ChatMessage } from "../types/chat";

type Props = {
    msg: ChatMessage;
};

function MessageBubble({ msg }: Props) {
    return (
    <div
    className={`message-row ${
        msg.role === "user" ? "user-row" : "bot-row"
    }`}
    >
    <div className={`message ${msg.role}`}>
        <ReactMarkdown>{msg.text}</ReactMarkdown>

        {msg.files && msg.files.length > 0 && (
        <div className="file-links">
            {msg.files.map((file, index) => (
            <a
                key={index}
                href={file.url}
                target="_blank"
                rel="noreferrer"
                className="file-link"
            >
                📄 {file.fileName}
            </a>
            ))}
        </div>
        )}
    </div>
    </div>
);
}

export default MessageBubble;