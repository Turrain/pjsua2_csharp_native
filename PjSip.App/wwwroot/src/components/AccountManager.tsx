import { createSignal, createResource, For, Show, Component } from 'solid-js';
import { api } from '../services/api';
import { AgentConfig, SipAccount, LLMConfig, WhisperConfig, AuralisConfig } from '../types';


interface CallDialogProps {
  accountId: string;
  onCall: (destination: string) => void;
  onClose: () => void;
}

const CallDialog: Component<CallDialogProps> = (props) => {
  const [destination, setDestination] = createSignal('');

  const handleSubmit = (e: Event) => {
    e.preventDefault();
    props.onCall(destination());
    props.onClose();
  };

  return (
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
      <div class="bg-gray-800 p-6 rounded-lg w-96">
        <h3 class="text-xl font-bold mb-4">Make Call</h3>
        <form onSubmit={handleSubmit} class="space-y-4">
          <input
            type="text"
            placeholder="sip:user@domain.com"
            class="w-full bg-gray-700 rounded-lg p-2"
            value={destination()}
            onInput={(e) => setDestination(e.currentTarget.value)}
            required
          />
          <div class="flex justify-end gap-2">
            <button
              type="button"
              class="px-4 py-2 bg-gray-700 rounded-lg"
              onClick={props.onClose}
            >
              Cancel
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 rounded-lg"
            >
              Call
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};




export default function AccountManager() {
  const [selectedTab, setSelectedTab] = createSignal<'accounts' | 'agents'>('accounts');
  const [accounts, { refetch: refetchAccounts }] = createResource(() => api.getAccounts());
  const [agents, { refetch: refetchAgents }] = createResource(() => api.getAgentConfigs());
  
  const [callDialogOpen, setCallDialogOpen] = createSignal(false);
  const [selectedAccount, setSelectedAccount] = createSignal<string | null>(null);
  const handleMakeCall = async (accountId: string, destination: string) => {
    try {
      const call = await api.makeCall(accountId, destination);
      console.log("Call initiated:", call);
      // Optionally add UI feedback for call initiation
    } catch (error) {
      console.error("Call failed:", error);
    }
  };
  // Account form state
  const [newAccount, setNewAccount] = createSignal<Omit<SipAccount, 'accountId' | 'isActive'> & { password: string }>({
    username: '',
    domain: '',
    registrarUri: '',
    password: '',
  });

  const handleClearAccounts = async () => {
    try {
      await api.clearAccounts();
      refetchAccounts();
      console.log("Accounts cleared.");
    } catch (error) {
      console.error("Failed to clear accounts:", error);
    }
  };
  
  const handleClearAgentConfigs = async () => {
    try {
      await api.clearAgentConfigs();
      refetchAgents();
      console.log("Agent configurations cleared.");
    } catch (error) {
      console.error("Failed to clear agent configurations:", error);
    }
  };
  const handleDeleteAccount = async (accountId: string) => {
    try {
      await api.deleteAccount(accountId);
      refetchAccounts();
      console.log(`Account ${accountId} deleted.`);
    } catch (error) {
      console.error("Failed to delete account:", error);
    }
  };
  
  const handleDeleteAgent = async (agentId: number) => {
    try {
      await api.deleteAgentConfig(agentId);
      refetchAgents();
      console.log(`Agent configuration ${agentId} deleted.`);
    } catch (error) {
      console.error("Failed to delete agent configuration:", error);
    }
  };
  const handleUpdateAgent = async (accountId: string, agentConfigId: number) => {
    try {
      await api.updateAccountAgent(accountId, agentConfigId);
      await refetchAccounts();
    } catch (error) {
      console.error('Failed to update agent:', error);
      // You might want to add proper error handling/notification here
    }
  };
  // Agent form state with proper nesting matching C# model
  const [newAgent, setNewAgent] = createSignal<Omit<AgentConfig, 'id'>>({
    agentId: '',
    llm: {
      model: 'llama2',
      temperature: 0.7,
      maxTokens: 512,
      ollamaEndpoint: 'http://localhost:11434',
      parameters: {}
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
    priority: 50,
    isEnabled: true,
  });

  // Form handlers
  const handleAccountSubmit = async (e: Event) => {
    e.preventDefault();
    await api.registerAccount(newAccount());
    refetchAccounts();
    setNewAccount({
      username: '',
      domain: '',
      registrarUri: '',
      password: '',
    });
  };

  const handleAgentSubmit = async (e: Event) => {
    e.preventDefault();
    await api.createAgentConfig(newAgent());
    refetchAgents();
    resetAgentForm();
  };

  const resetAgentForm = () => {
    setNewAgent({
      ...newAgent(),
      agentId: '',
    });
  };

  // Update handlers for nested configuration objects
  const updateLLMConfig = (update: Partial<LLMConfig>) => {
    setNewAgent({
      ...newAgent(),
      llm: { ...newAgent().llm, ...update }
    });
  };

  const updateWhisperConfig = (update: Partial<WhisperConfig>) => {
    setNewAgent({
      ...newAgent(),
      whisper: { ...newAgent().whisper, ...update }
    });
  };

  const updateAuralisConfig = (update: Partial<AuralisConfig>) => {
    setNewAgent({
      ...newAgent(),
      auralis: { ...newAgent().auralis, ...update }
    });
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
                required
              />
              <input
                type="password"
                placeholder="Password"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().password}
                onInput={(e) => setNewAccount({ ...newAccount(), password: e.currentTarget.value })}
                required
              />
              <input
                type="text"
                placeholder="Domain"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().domain}
                onInput={(e) => setNewAccount({ ...newAccount(), domain: e.currentTarget.value })}
                required
              />
              <input
                type="text"
                placeholder="Registrar URI"
                class="bg-gray-700 rounded-lg p-2"
                value={newAccount().registrarUri}
                onInput={(e) => setNewAccount({ ...newAccount(), registrarUri: e.currentTarget.value })}
                required
              />
              <button type="submit" class="col-span-2 bg-blue-600 rounded-lg p-2 hover:bg-blue-700 transition">
                Register Account
              </button>
            </form>
          </div>

          <div class="bg-gray-800 rounded-xl p-6">
          <div class="mt-4 flex justify-end">
      <button
        class="px-4 py-2 bg-red-600 rounded-lg hover:bg-red-700 transition"
        onClick={handleClearAccounts}
      >
        Clear All Accounts
      </button>
    </div>
            <h2 class="text-2xl font-bold mb-4">Active SIP Accounts</h2>
            <div class="overflow-x-auto">
              <table class="w-full">
                <thead class="bg-gray-700">
                  <tr>
                    <th class="p-3 text-left">ID</th>
                    <th class="p-3 text-left">Username</th>
                    <th class="p-3 text-left">Domain</th>
                    <th class="p-3 text-left">Status</th>
                    <th class="p-3 text-left">Linked Agent</th>
                    <th class="p-3 text-left">Action</th>
                  </tr>
                </thead>
                <tbody>
                <For each={accounts()}>
    {(account) => (
      <tr class="hover:bg-gray-700 transition">
        <td class="p-3">{account.accountId}</td>
        <td class="p-3">{account.username}</td>
        <td class="p-3">{account.domain}</td>
        <td class="p-3">
          <span class={`px-2 py-1 rounded-full ${account.isActive ? 'bg-green-500' : 'bg-red-500'}`}>
            {account.isActive ? 'Active' : 'Inactive'}
          </span>
        </td>
        <td class="p-3">
          <select
            class="bg-gray-700 rounded-lg p-2 w-full"
            value={account.agent?.id || ''}
            onChange={(e) => handleUpdateAgent(account.accountId, parseInt(e.currentTarget.value))}
          >
            <option value="">Select Agent</option>
            <For each={agents()}>
              {(agent) => (
                <option value={agent.id}>{agent.agentId}</option>
              )}
            </For>
          </select>
        </td>
        <td class="p-3">
                          <button
                            class="px-2 py-1 bg-blue-500 rounded hover:bg-blue-600"
                            onClick={() => {
                              setSelectedAccount(account.accountId);
                              setCallDialogOpen(true);
                            }}
                          >
                            Call
                          </button>
                        </td>
                        <td class="p-3">
  <button
    class="px-2 py-1 bg-red-500 rounded hover:bg-red-600"
    onClick={() => handleDeleteAccount(account.accountId)}
  >
    Delete
  </button>
</td>
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
            <form onSubmit={handleAgentSubmit} class="grid grid-cols-1 gap-6">
              <div class="grid grid-cols-2 gap-4">
                <input
                  type="text"
                  placeholder="Agent ID"
                  class="bg-gray-700 rounded-lg p-2"
                  value={newAgent().agentId}
                  onInput={(e) => setNewAgent({ ...newAgent(), agentId: e.currentTarget.value })}
                  required
                />
                <div class="flex gap-4">
                  <input
                    type="number"
                    placeholder="Priority (0-100)"
                    class="bg-gray-700 rounded-lg p-2 flex-1"
                    min="0"
                    max="100"
                    value={newAgent().priority}
                    onInput={(e) => setNewAgent({ ...newAgent(), priority: Number(e.currentTarget.value) })}
                  />
                  <label class="flex items-center gap-2">
                    <input
                      type="checkbox"
                      checked={newAgent().isEnabled}
                      onChange={(e) => setNewAgent({ ...newAgent(), isEnabled: e.currentTarget.checked })}
                    />
                    <span>Enabled</span>
                  </label>
                </div>
              </div>
              
              {/* LLM Configuration */}
              <div class="bg-gray-700 p-4 rounded-lg">
                <h3 class="font-bold mb-4">LLM Configuration</h3>
                <div class="grid grid-cols-2 gap-4">
                  <input
                    type="text"
                    placeholder="Model"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().llm.model}
                    onInput={(e) => updateLLMConfig({ model: e.currentTarget.value })}
                    required
                  />
                  <input
                    type="url"
                    placeholder="Ollama Endpoint"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().llm.ollamaEndpoint}
                    onInput={(e) => updateLLMConfig({ ollamaEndpoint: e.currentTarget.value })}
                    required
                  />
                  <div>
                    <label class="block text-sm mb-1">Temperature (0-2)</label>
                    <input
                      type="range"
                      min="0"
                      max="2"
                      step="0.1"
                      class="w-full"
                      value={newAgent().llm.temperature}
                      onInput={(e) => updateLLMConfig({ temperature: parseFloat(e.currentTarget.value) })}
                    />
                    <div class="text-right">{newAgent().llm.temperature}</div>
                  </div>
                  <input
                    type="number"
                    placeholder="Max Tokens (1-4096)"
                    class="bg-gray-600 rounded-lg p-2"
                    min="1"
                    max="4096"
                    value={newAgent().llm.maxTokens}
                    onInput={(e) => updateLLMConfig({ maxTokens: parseInt(e.currentTarget.value) })}
                  />
                </div>
              </div>

              {/* Whisper Configuration */}
              <div class="bg-gray-700 p-4 rounded-lg">
                <h3 class="font-bold mb-4">Whisper Configuration</h3>
                <div class="grid grid-cols-2 gap-4">
                  <input
                    type="url"
                    placeholder="Endpoint"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().whisper.endpoint}
                    onInput={(e) => updateWhisperConfig({ endpoint: e.currentTarget.value })}
                    required
                  />
                  <input
                    type="text"
                    placeholder="Language Code"
                    class="bg-gray-600 rounded-lg p-2"
                    maxLength={5}
                    value={newAgent().whisper.language}
                    onInput={(e) => updateWhisperConfig({ language: e.currentTarget.value })}
                  />
                  <input
                    type="number"
                    placeholder="Timeout (seconds)"
                    class="bg-gray-600 rounded-lg p-2"
                    min="1"
                    max="60"
                    value={newAgent().whisper.timeout}
                    onInput={(e) => updateWhisperConfig({ timeout: parseInt(e.currentTarget.value) })}
                  />
                  <label class="flex items-center gap-2">
                    <input
                      type="checkbox"
                      checked={newAgent().whisper.enableTranslation}
                      onChange={(e) => updateWhisperConfig({ enableTranslation: e.currentTarget.checked })}
                    />
                    <span>Enable Translation</span>
                  </label>
                </div>
              </div>

              {/* Auralis Configuration */}
              <div class="bg-gray-700 p-4 rounded-lg">
                <h3 class="font-bold mb-4">Auralis Configuration</h3>
                <div class="grid grid-cols-2 gap-4">
                  <input
                    type="url"
                    placeholder="Endpoint"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().auralis.endpoint}
                    onInput={(e) => updateAuralisConfig({ endpoint: e.currentTarget.value })}
                    required
                  />
                  <input
                    type="password"
                    placeholder="API Key"
                    class="bg-gray-600 rounded-lg p-2"
                    value={newAgent().auralis.apiKey}
                    onInput={(e) => updateAuralisConfig({ apiKey: e.currentTarget.value })}
                    required
                  />
                  <input
                    type="number"
                    placeholder="Timeout (seconds)"
                    class="bg-gray-600 rounded-lg p-2"
                    min="1"
                    max="60"
                    value={newAgent().auralis.timeout}
                    onInput={(e) => updateAuralisConfig({ timeout: parseInt(e.currentTarget.value) })}
                  />
                  <label class="flex items-center gap-2">
                    <input
                      type="checkbox"
                      checked={newAgent().auralis.enableAnalytics}
                      onChange={(e) => updateAuralisConfig({ enableAnalytics: e.currentTarget.checked })}
                    />
                    <span>Enable Analytics</span>
                  </label>
                </div>
              </div>

              <button type="submit" class="bg-blue-600 rounded-lg p-2 hover:bg-blue-700 transition">
                Create Agent Configuration
              </button>
            </form>
          </div>

          <div class="bg-gray-800 rounded-xl p-6">
          <div class="mt-4 flex justify-end">
      <button
        class="px-4 py-2 bg-red-600 rounded-lg hover:bg-red-700 transition"
        onClick={handleClearAgentConfigs}
      >
        Clear All Agent Configurations
      </button>
    </div>
            <h2 class="text-2xl font-bold mb-4">Agent Configurations</h2>
            <div class="overflow-x-auto">
              <table class="w-full">
                <thead class="bg-gray-700">
                  <tr>
                    <th class="p-3 text-left">ID</th>
                    <th class="p-3 text-left">Agent ID</th>
                    <th class="p-3 text-left">LLM Model</th>
                    <th class="p-3 text-left">Priority</th>
                    <th class="p-3 text-left">Status</th>
                    <th class="p-3 text-left">Created</th>
                    <th class="p-3 text-left">Updated</th>
                  </tr>
                </thead>
                <tbody>
                  <For each={agents()}>
                    {(agent) => (
                      <tr class="hover:bg-gray-700 transition">
                        <td class="p-3">{agent.id}</td>
                        <td class="p-3">{agent.agentId}</td>
                        <td class="p-3">{agent.llm.model}</td>
                        <td class="p-3">{agent.priority}</td>
                        <td class="p-3">
                          <span class={`px-2 py-1 rounded-full ${agent.isEnabled ? 'bg-green-500' : 'bg-red-500'}`}>
                            {agent.isEnabled ? 'Enabled' : 'Disabled'}
                          </span>
                        </td>
                        <td class="p-3">{agent.createdAt ? new Date(agent.createdAt).toLocaleString() : '-'}</td>
                        <td class="p-3">{agent.updatedAt ? new Date(agent.updatedAt).toLocaleString() : '-'}</td>
                        <td class="p-3">
  <button
    class="px-2 py-1 bg-red-500 rounded hover:bg-red-600"
    onClick={() => handleDeleteAgent(agent.id)}
  >
    Delete
  </button>
</td>
                      </tr>
                    )}
                  </For>
                </tbody>
              </table>
            </div>
          </div>
        </Show>
      </div>
      <Show when={callDialogOpen()}>
        <CallDialog 
          accountId={selectedAccount()!}
          onCall={(destination: string) => handleMakeCall(selectedAccount()!, destination)}
          onClose={() => setCallDialogOpen(false)}
        />
      </Show>
    </div>
  );
}