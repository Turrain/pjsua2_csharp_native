// src/types.ts
export type SipAccount = {
    id: string;
    username: string;
    domain: string;
    registrarUri: string;
    isActive: boolean;
  };
  
  export type Call = {
    callId: number;
    status: string;
    remoteUri: string;
    accountId: string;
    startTime: string;
  };