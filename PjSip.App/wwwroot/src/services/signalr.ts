import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Call, SipAccount } from '../types';

// Interface definitions for Call and SipAccount (adjust according to your needs)

class SignalRService {
    private connection: HubConnection;
    private isConnected: boolean = false;
    private readonly hubUrl: string = 'http://127.0.0.1:5000/siphub';

    constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.setupConnectionHandlers();
    }

    // Start the SignalR connection
    public async start(): Promise<void> {
        if (this.isConnected) {
            console.log('Already connected');
            return;
        }

        try {
            console.log('Starting connection to', this.hubUrl);
            await this.connection.start();
            this.isConnected = true;
            console.log('Connected! Connection ID:', this.connection.connectionId);
            // Test server responsiveness
            const pingResponse = await this.connection.invoke('Ping');
            console.log('Ping response:', pingResponse);
        } catch (error) {
            console.error('Connection failed:', error);
            this.isConnected = false;
        }
    }

    // Stop the SignalR connection
    public async stop(): Promise<void> {
        if (!this.isConnected) return;
        await this.connection.stop();
        this.isConnected = false;
        console.log('Connection stopped');
    }

    // Subscribe to call updates
    public onCallUpdate(callback: (call: Call) => void): void {
        this.connection.on('CallUpdate', (call: Call) => {
            console.log('Received CallUpdate:', call);
            callback(call);
        });
    }

    public onAccountUpdate(callback: (account: SipAccount) => void): void {
        this.connection.on('AccountUpdate', (account: SipAccount) => {
            console.log('Received AccountUpdate:', account);
            callback(account);
        });
    }

    // Unsubscribe from call updates
    private setupConnectionHandlers(): void {
        this.connection.onclose((error) => {
            this.isConnected = false;
            console.error('Connection closed:', error);
        });

        this.connection.onreconnecting((error) => {
            console.log('Reconnecting:', error);
        });

        this.connection.onreconnected((connectionId) => {
            this.isConnected = true;
            console.log('Reconnected with Connection ID:', connectionId);
        });
    }

    public getConnectionStatus(): boolean {
        return this.isConnected;
    }

  
}

// Singleton instance
const signalRService = new SignalRService();
export default signalRService;