import { Component, For, Show, createResource, createSignal, onMount } from 'solid-js';
import { api } from '../services/api';
import { SipAccount } from '../types';
import { AccountCallList } from './AccountCallList';
import { NewAccountForm } from './NewAccountForm';
import signalRService from '../services/signalr';

interface AccountsPanelProps {
  onMakeCall: (accountId: string) => void;
}

export const AccountsPanel: Component<AccountsPanelProps> = (props) => {
  const [accounts, { refetch }] = createResource(() => api.getAccounts());
  onMount(async () => {
    console.log('onMount triggered'); // Confirm mount
    try {
      await signalRService.start();
      console.log('SignalR connection status:', signalRService.getConnectionStatus());
      signalRService.onCallUpdate((call) => {
        console.log('Call update handler set');
        refetch();
      });
      signalRService.onAccountUpdate((account) => {
        console.log('Account update handler set, account:', account);
        refetch();
      });
    } catch (error) {
      console.error('Error in onMount:', error);
    }
  });
  const handleDeleteAccount = async (accountId: string) => {
    try {
      await api.deleteAccount(accountId);
      refetch();
    } catch (error) {
      console.error('Failed to delete account:', error);
    }
  };

  const handleClearAccounts = async () => {
    try {
      await api.clearAccounts();
      refetch();
    } catch (error) {
      console.error('Failed to clear accounts:', error);
    }
  };

  return (
    <>
      {/* New Account Form */}
      <NewAccountForm onSubmit={refetch} />
      
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
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <For each={accounts()}>
            {(account: SipAccount) => (
              <div class="bg-gray-900 rounded-xl p-4 shadow">
                <div class="flex justify-between items-center">
                  <h3 class="text-xl font-semibold">{account.username}</h3>
                  <span
                    class={`px-2 py-1 rounded-full ${account.isActive ? 'bg-green-500' : 'bg-red-500'}`}
                  >
                    {account.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
                <div class="mt-2 text-sm">
                  <p><strong>ID:</strong> {account.accountId}</p>
                  <p><strong>Domain:</strong> {account.domain}</p>
                  <p>
                    <strong>Linked Agent:</strong>{' '}
                    {account.agent ? account.agent.agentId : 'None'}
                  </p>
                </div>
                <div class="mt-4 flex gap-2">
                  <button
                    class="px-2 py-1 bg-blue-500 rounded hover:bg-blue-600 transition"
                    onClick={() => props.onMakeCall(account.accountId)}
                  >
                    Call
                  </button>
                  <button
                    class="px-2 py-1 bg-red-500 rounded hover:bg-red-600 transition"
                    onClick={() => handleDeleteAccount(account.accountId)}
                  >
                    Delete
                  </button>
                </div>
                <AccountCallList account={account} refresh={refetch} />
              </div>
            )}
          </For>
        </div>
      </div>
    </>
  );
};