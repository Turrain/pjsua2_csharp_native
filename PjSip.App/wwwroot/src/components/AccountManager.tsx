import { createSignal, createResource, Show } from 'solid-js';
import { api } from '../services/api';
import { AccountManagerTabs } from './AccountManagerTabs';
import { AccountsPanel } from './AccountPanel';
import { CallDialog } from './CallDialog';

export default function AccountManager() {
  const [selectedTab, setSelectedTab] = createSignal<'accounts' | 'agents'>('accounts');
  const [callDialogOpen, setCallDialogOpen] = createSignal(false);
  const [selectedAccount, setSelectedAccount] = createSignal<string | null>(null);

  const handleMakeCall = async (accountId: string, destination: string) => {
    try {
      const call = await api.makeCall(accountId, destination);
      console.log('Call initiated:', call);
    } catch (error) {
      console.error('Call failed:', error);
    }
  };

  return (
    <div class="min-h-screen bg-gray-900 text-gray-100 p-8">
      <div class="max-w-6xl mx-auto">
        <AccountManagerTabs
          selectedTab={selectedTab()}
          onTabChange={setSelectedTab}
        />
        
        <Show when={selectedTab() === 'accounts'}>
          <AccountsPanel
            onMakeCall={(accountId) => {
              setSelectedAccount(accountId);
              setCallDialogOpen(true);
            }}
          />
        </Show>
        
        {/* Agents panel would go here in a similar separate component */}
        
        <Show when={callDialogOpen()}>
          <CallDialog
            accountId={selectedAccount()!}
            onCall={(destination) => handleMakeCall(selectedAccount()!, destination)}
            onClose={() => setCallDialogOpen(false)}
          />
        </Show>
      </div>
    </div>
  );
}