// src/services/api.ts
import { SipAccount, Call, AgentConfig } from '../types';

export const api = {
  async registerAccount(account: Omit<SipAccount, 'accountId' | 'isActive'> & { password: string }) {
    const response = await fetch('/api/SipAccounts', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(account)
    });
    if (!response.ok) throw new Error('Failed to register account');
    return response.json();
  },
  async clearAccounts() {
    const response = await fetch('/api/SipAccounts', { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to clear accounts');
    return response.json();
  },
  async clearAgentConfigs() {
    const response = await fetch('/api/AgentConfig', { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to clear agent configurations');
    return response.json();
  },
  async deleteAccount(accountId: string) {
    const response = await fetch(`/api/SipAccounts/${accountId}`, { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to delete account');
    return response.json();
  },

  async deleteAgentConfig(id: number) {
    const response = await fetch(`/api/AgentConfig/${id}`, { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to delete agent configuration');
    return response.json();
  },
  async getCalls() {
    const response = await fetch('/api/SipCall'); // ensure your backend exposes this endpoint
    if (!response.ok) throw new Error('Failed to fetch calls');
    return response.json() as Promise<Call[]>;
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
  async updateAccountAgent(accountId: string, agentConfigId: number) {
    const response = await fetch(`/api/SipAccounts/${accountId}/agent`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ agentConfigId })
    });
    if (!response.ok) throw new Error('Failed to update account agent');
    return response.json() as Promise<SipAccount>;
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
    const response = await fetch(`/api/sipcall/${callId}`, { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to hang up call');
    return response.json();
},
};