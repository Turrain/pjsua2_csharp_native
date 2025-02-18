// src/App.tsx
import { createSignal, createEffect, Show } from 'solid-js';
import { api } from './services/api';
import { SipAccount, Call } from './types';
import  './App.css';

export default function App() {
  const [accounts, setAccounts] = createSignal<SipAccount[]>([]);
  const [currentCall, setCurrentCall] = createSignal<Call | null>(null);
  const [isRegistering, setIsRegistering] = createSignal(false);
  const [error, setError] = createSignal<string | null>(null);

  // Form state
  const [formData, setFormData] = createSignal({
    username: '',
    password: '',
    domain: '',
    registrarUri: ''
  });

  const fetchAccounts = async () => {
    try {
      const accounts = await api.getAccounts();
      setAccounts(accounts);
    } catch (err) {
      setError('Failed to fetch accounts');
    }
  };

  createEffect(() => {
    fetchAccounts();
  });

  const handleRegister = async (e: Event) => {
    e.preventDefault();
    setIsRegistering(true);
    setError(null);
    
    try {
      await api.registerAccount(formData());
      await fetchAccounts();
      setFormData({
        username: '',
        password: '',
        domain: '',
        registrarUri: ''
      });
    } catch (err) {
      setError('Failed to register account');
    } finally {
      setIsRegistering(false);
    }
  };

  const handleMakeCall = async (accountId: string, destination: string) => {
    try {
      const call = await api.makeCall(accountId, destination);
      setCurrentCall(call);
    } catch (err) {
      setError('Failed to make call');
    }
  };

  const handleHangup = async () => {
    const call = currentCall();
    if (!call) return;

    try {
      await api.hangupCall(call.callId);
      setCurrentCall(null);
    } catch (err) {
      setError('Failed to hangup call');
    }
  };

  return (
    <div class="container">
      <h1>SIP Client</h1>

      <Show when={error()}>
      <div class="error">{error()}</div>
      </Show>

      <section class="registerForm">
      <h2>Register Account</h2>
      <form onSubmit={handleRegister}>
        <div>
        <label>Username:</label>
        <input 
          type="text"
          value={formData().username}
          onInput={e => setFormData({...formData(), username: e.currentTarget.value})}
        />
        </div>
        <div>
        <label>Password:</label>
        <input 
          type="password"
          value={formData().password}
          onInput={e => setFormData({...formData(), password: e.currentTarget.value})}
        />
        </div>
        <div>
        <label>Domain:</label>
        <input 
          type="text"
          value={formData().domain}
          onInput={e => setFormData({...formData(), domain: e.currentTarget.value})}
        />
        </div>
        <div>
        <label>Registrar URI:</label>
        <input 
          type="text"
          value={formData().registrarUri}
          onInput={e => setFormData({...formData(), registrarUri: e.currentTarget.value})}
        />
        </div>
        <button type="submit" disabled={isRegistering()}>
        {isRegistering() ? 'Registering...' : 'Register'}
        </button>
      </form>
      </section>

      <section class="accounts">
      <h2>SIP Accounts</h2>
      <div class="accountsList">
        {accounts().map(account => (
        <div class="accountCard">
          <div>Username: {account.username}</div>
          <div>Domain: {account.domain}</div>
          <div>Status: {account.isActive ? 'Active' : 'Inactive'}</div>
          <Show when={account.isActive}>
          <div class="dialForm">
            <input 
            type="text" 
            placeholder="Enter destination URI"
            onKeyPress={e => {
              if (e.key === 'Enter') {
              handleMakeCall(account.id, e.currentTarget.value);
              e.currentTarget.value = '';
              }
            }}
            />
          </div>
          </Show>
        </div>
        ))}
      </div>
      </section>

      <Show when={currentCall()}>
      <section class="currentCall">
        <h2>Active Call</h2>
        <div class="callInfo">
        <div>Remote URI: {currentCall()?.remoteUri}</div>
        <div>Status: {currentCall()?.status}</div>
        <button onClick={handleHangup} class="hangupButton">
          Hangup
        </button>
        </div>
      </section>
      </Show>
    </div>
  );
}