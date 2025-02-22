import { Component, createSignal } from 'solid-js';
import { api } from '../services/api';
import { SipAccount } from '../types';

interface NewAccountFormProps {
  onSubmit: () => void;
}

export const NewAccountForm: Component<NewAccountFormProps> = (props) => {
  const [newAccount, setNewAccount] = createSignal<
    Omit<SipAccount, 'accountId' | 'isActive'> & { password: string }
  >({
    username: '',
    domain: 'localhost',
    registrarUri: 'sip:localhost',
    password: '',
    calls: [],
  });

  const handleSubmit = async (e: Event) => {
    e.preventDefault();
    await api.registerAccount(newAccount());
    props.onSubmit();
    setNewAccount({
      username: '',
      domain: 'localhost',
      registrarUri: 'sip:localhost',
      password: '',
      calls: [],
    });
  };

  return (
    <div class="bg-gray-800 rounded-xl p-6 mb-8">
      <h2 class="text-2xl font-bold mb-4">Register New SIP Account</h2>
      <form onSubmit={handleSubmit} class="grid grid-cols-2 gap-4">
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
        <button
          type="submit"
          class="col-span-2 bg-blue-600 rounded-lg p-2 hover:bg-blue-700 transition"
        >
          Register Account
        </button>
      </form>
    </div>
  );
};