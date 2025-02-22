export interface LLMConfig {
  model: string;
  temperature: number;
  maxTokens: number;
  ollamaEndpoint: string;
  parameters?: Record<string, string>;
}

export interface WhisperConfig {
  endpoint: string;
  language: string;
  timeout: number;
  enableTranslation: boolean;
}

export interface AuralisConfig {
  endpoint: string;
  apiKey: string;
  timeout: number;
  enableAnalytics: boolean;
}

export interface AgentConfig {
  id: number;
  agentId: string;
  llm: LLMConfig;
  whisper: WhisperConfig;
  auralis: AuralisConfig;
  createdAt?: string;
  updatedAt?: string;
  priority: number;
  isEnabled: boolean;
}

// Existing types
export interface SipAccount {
  accountId: string; // Changed from id to accountId
  username: string;
  domain: string;
  registrarUri: string;
  isActive: boolean;
  agent?: AgentConfig;
  calls: Call[];
}
  
export interface Call {
  callId: number;
  remoteUri: string;
  status: string;
  startedAt: string;
  endedAt?: string;
}