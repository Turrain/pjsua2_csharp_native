import { Component } from 'solid-js';

interface TabProps {
  selectedTab: 'accounts' | 'agents';
  onTabChange: (tab: 'accounts' | 'agents') => void;
}

export const AccountManagerTabs: Component<TabProps> = (props) => (
  <div class="flex gap-4 mb-8">
    <button
      class={`px-4 py-2 rounded-lg ${props.selectedTab === 'accounts' ? 'bg-blue-600' : 'bg-gray-700'}`}
      onClick={() => props.onTabChange('accounts')}
    >
      SIP Accounts
    </button>
    <button
      class={`px-4 py-2 rounded-lg ${props.selectedTab === 'agents' ? 'bg-blue-600' : 'bg-gray-700'}`}
      onClick={() => props.onTabChange('agents')}
    >
      Agent Configurations
    </button>
  </div>
);