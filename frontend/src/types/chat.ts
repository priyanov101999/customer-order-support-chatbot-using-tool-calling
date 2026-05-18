export type FileLink = {
    fileName: string;
    url: string;
};

export type ChatMessage = {
    role: "user" | "bot";
    text: string;
    files?: FileLink[];
};

export type ChatResponse = {
    toolUsed?: string;
    data?: any;
    message?: string;
    files?: FileLink[];
};