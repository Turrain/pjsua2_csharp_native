// src/services/api.ts
import { SipAccount, Call, AgentConfig } from '../types';

export const api = {
  async registerAccount(account: Omit<SipAccount, 'id' | 'isActive'> & { password: string }) {
    const response = await fetch('/api/SipAccounts', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(account)
    });
    if (!response.ok) throw new Error('Failed to register account');
    return response.json();
  },
  async createAgentConfig(config: Omit<AgentConfig, 'id'>) {
    const response = await fetch('/api/AgentConfig', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(config)
    });
    if (!response.ok) throw new Error('Failed to create agent config');
    return response.json();
  },

  async getAgentConfigs() {
    const response = await fetch('/api/AgentConfig');
    if (!response.ok) throw new Error('Failed to fetch agent configs');
    return response.json() as Promise<AgentConfig[]>;
  },
  async getAccounts() {
    const response = await fetch('/api/SipAccounts');
    if (!response.ok) throw new Error('Failed to fetch accounts');
    return response.json() as Promise<SipAccount[]>;
  },


  async makeCall(accountId: string, destination: string) {
    const response = await fetch('/api/SipCall', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ accountId, destination })
    });
    if (!response.ok) throw new Error('Failed to make call');
    return response.json() as Promise<Call>;
  },

  async hangupCall(callId: number) {
    const response = await fetch(`/api/SipCall/${callId}/hangup`, {
      method: 'POST'
    });
    if (!response.ok) throw new Error('Failed to hangup call');
  }
};