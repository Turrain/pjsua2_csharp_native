

import { onMount } from 'solid-js'
import AccountManager from './components/AccountManager';
import signalRService from './services/signalr';

export default function App() {
  

  return (
    <div class="">
      <AccountManager/>
    </div>
  );
}