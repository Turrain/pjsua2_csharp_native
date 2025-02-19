// src/components/AccountManager.tsx
import { createSignal, createResource, For, Show } from 'solid-js';
import { api} from '../services/api';
import { AgentConfig, SipAccount } from '../types';

export default function AccountManager() {
  const [selectedTab, setSelectedTab] = createSignal<'accounts' | 'agents'>('accounts');
  const [newAccount, setNewAccount] = createSignal<Omit<SipAccount, 'id' | 'isActive'> & { password: string }>({
    username: '',
    domain: '',
    registrarUri: '',
    password: '',
  });
  
  const [newAgent, setNewAgent] = createSignal<Omit<AgentConfig, 'id'>>({
    agentId: '',
    llm: {
      model: 'llama2',
      temperature: 0.7,
      maxTokens: 2000,
      ollamaEndpoint: 'http://localhost:11434',
    },
    whisper: {
      endpoint: 'https://api.whisper.ai/v1',
      language: 'en',
      timeout: 30,
      enableTranslation: false,
    },
    auralis: {
      endpoint: 'https://api.auralis.cloud/v1',
      apiKey: '',
      timeout: 30,
      enableAnalytics: true,
    },
    priority: 1,
    isEnabled: true,
  });

  const [accounts, { refetch: refetchAccounts }] = createResource(() => api.getAccounts());
  const [agents, { refetch: refetchAgents }] = createResource(() => api.getAgentConfigs());

  const handleAccountSubmit = async (e: Event) => {
    e.preventDefault();
    await api.registerAccount(newAccount());
    refetchAccounts();
    setNewAccount({ username: '', domain: '', registrarUri: '', password: '' });
  };

  const handleAgentSubmit = async (e: Event) => {
    e.preventDefault();
    await api.createAgentConfig(newAgent());
    refetchAgents();
    // Reset form
    setNewAgent({ ...newAgent(), agentId: '', priority: 1 });
  };

  return (
    <div class="min-h-screen bg-gray-900 text-gray-100 p-8">
      <div class="max-w-6xl mx-auto">
        <div class="flex gap-4 mb-8">
          <button
            class={`px-4 py-2 rounded-lg ${selectedTab() === 'accounts' ? 'bg-blue-600' : 'bg-gray-700'}`}
            onClick={() => setSelectedTab('accounts')}
          >
            SIP Accounts
          </button>
          <button
            class={`px-4 py-2 rounded-lg ${selectedTab() === 'agents' ? 'bg-blue-600' : 'bg-gray-700'}`}
            onClick={() => setSelectedTab('agents')}
          >
            Agent Configurations
          </button>
        </div>

        <Show when={selectedTab() === 'accounts'}>
          <div class="bg-gray-800 rounded-xl p-6 mb-8">
            <h2 class="text-2xl font-bold mb-4">Register New SIP Account</h2>
            <form onSubmit={handleAccountSubmit} class="grid grid-cols-2 gap-4">
              <input
                type="text"
                placeholder="Username"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().username}
                onInput={(e) => setNewAccount({ ...newAccount(), username: e.currentTarget.value })}
              />
              <input
                type="password"
                placeholder="Password"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().password}
                onInput={(e) => setNewAccount({ ...newAccount(), password: e.currentTarget.value })}
              />
              <input
                type="text"
                placeholder="Domain"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().domain}
                onInput={(e) => setNewAccount({ ...newAccount(), domain: e.currentTarget.value })}
              />
              <input
                type="text"
                placeholder="Registrar URI"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().registrarUri}
                onInput={(e) => setNewAccount({ ...newAccount(), registrarUri: e.currentTarget.value })}
              />
              <button type="submit" class="col-span-2 bg-blue-600 rounded-lg p-2 hover:bg-blue-700 transition">
                Register Account
              </button>
            </form>
          </div>

          <div class="bg-gray-800 rounded-xl p-6">
            <h2 class="text-2xl font-bold mb-4">Active SIP Accounts</h2>
            <div class="overflow-x-auto">
              <table class="w-full">
                <thead class="bg-gray-700">
                  <tr>
                    <th class="p-3 text-left">Username</th>
                    <th class="p-3 text-left">Domain</th>
                    <th class="p-3 text-left">Status</th>
                    <th class="p-3 text-left">Linked Agent</th>
                  </tr>
                </thead>
                <tbody>
                  <For each={accounts()}>
                    {(account) => (
                      <tr class="hover:bg-gray-700 transition">
                        <td class="p-3">{account.username}</td>
                        <td class="p-3">{account.domain}</td>
                        <td class="p-3">
                          <span class={`px-2 py-1 rounded-full ${account.isActive ? 'bg-green-500' : 'bg-red-500'}`}>
                            {account.isActive ? 'Active' : 'Inactive'}
                          </span>
                        </td>
                        <td class="p-3">{account.agent?.agentId || 'None'}</td>
                      </tr>
                    )}
                  </For>
                </tbody>
              </table>
            </div>
          </div>
        </Show>

        <Show when={selectedTab() === 'agents'}>
          <div class="bg-gray-800 rounded-xl p-6 mb-8">
            <h2 class="text-2xl font-bold mb-4">Create New Agent Configuration</h2>
            <form onSubmit={handleAgentSubmit} class="grid grid-cols-2 gap-4">
              <input
                type="text"
                placeholder="Agent ID"
                class="bg-gray-700 rounded-lg p-2"
                value={newAgent().agentId}
                onInput={(e) => setNewAgent({ ...newAgent(), agentId: e.currentTarget.value })}
              />
              <input
                type="number"
                placeholder="Priority"
                class="bg-gray-700 rounded-lg p-2"
                value={newAgent().priority}
                onInput={(e) => setNewAgent({ ...newAgent(), priority: Number(e.currentTarget.value) })}
              />
              
              <div class="col-span-2 bg-gray-700 p-4 rounded-lg">
                <h3 class="font-bold mb-2">LLM Configuration</h3>
                <div class="grid grid-cols-2 gap-4">
                  <input
                    type="text"
                    placeholder="Model"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().llm.model}
                    onInput={(e) => setNewAgent({ ...newAgent(), llm: { ...newAgent().llm, model: e.currentTarget.value } })}
                  />
                  <input
                    type="text"
                    placeholder="Ollama Endpoint"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().llm.ollamaEndpoint}
                    onInput={(e) => setNewAgent({ ...newAgent(), llm: { ...newAgent().llm, ollamaEndpoint: e.currentTarget.value } })}
                  />
                </div>
              </div>

              {/* Similar sections for Whisper and Auralis configurations */}

              <button type="submit" class="col-span-2 bg-blue-600 rounded-lg p-2 hover:bg-blue-700 transition">
                Create Agent Configuration
              </button>
            </form>
          </div>

          <div class="bg-gray-800 rounded-xl p-6">
            <h2 class="text-2xl font-bold mb-4">Agent Configurations</h2>
            <div class="overflow-x-auto">
              <table class="w-full">
                <thead class="bg-gray-700">
                  <tr>
                    <th class="p-3 text-left">Agent ID</th>
                    <th class="p-3 text-left">Priority</th>
                    <th class="p-3 text-left">Status</th>
                    <th class="p-3 text-left">Created</th>
                  </tr>
                </thead>
                <tbody>
                  <For each={agents()}>
                    {(agent) => (
                      <tr class="hover:bg-gray-700 transition">
                        <td class="p-3">{agent.agentId}</td>
                        <td class="p-3">{agent.priority}</td>
                        <td class="p-3">
                          <span class={`px-2 py-1 rounded-full ${agent.isEnabled ? 'bg-green-500' : 'bg-red-500'}`}>
                            {agent.isEnabled ? 'Enabled' : 'Disabled'}
                          </span>
                        </td>
                        <td class="p-3">{new Date(agent.createdAt || '').toLocaleDateString()}</td>
                      </tr>
                    )}
                  </For>
                </tbody>
              </table>
            </div>
          </div>
        </Show>
      </div>
    </div>
  );
}