import { Component, For, Show } from 'solid-js';
import { SipAccount, Call } from '../types';
import { api } from '../services/api';

interface AccountCallListProps {
  account: SipAccount;
  refresh: () => void;
}

export const AccountCallList: Component<AccountCallListProps> = (props) => {
  const activeCalls = () => props.account.calls.filter((call: Call) => !call.endedAt);

  const handleHangup = async (callId: number) => {
    try {
      await api.hangupCall(callId);
      props.refresh();
    } catch (error) {
      console.error('Failed to hang up call:', error);
    }
  };

  return (
    <Show when={activeCalls().length > 0}>
      <div class="mt-2">
        <h4 class="text-sm font-semibold mb-2">Active Calls</h4>
        <div class="space-y-2">
          <For each={activeCalls()}>
            {(call: Call) => (
              <div class="flex items-center justify-between px-3 py-2 bg-gray-700 rounded">
                <div>
                  <div class="text-sm">{call.remoteUri}</div>
                  <div class="text-xs text-gray-400">Status: {call.status}</div>
                </div>
                <button
                  onClick={() => handleHangup(call.callId)}
                  class="px-2 py-1 text-sm bg-red-600 hover:bg-red-700 rounded"
                >
                  Hang Up
                </button>
              </div>
            )}
          </For>
        </div>
      </div>
    </Show>
  );
};