
using PjSua2.Lx;
using PjSua2.Native;
using PjSua2.Native.pjsua2;
//GenHttp
class Program
{
    static async Task Main(string[] args)
    {
       
        using Manager mgr = new();
        // Add an account and wait for registration.
        var regStatus = await mgr.AddAccount(
            accountId: "1000",
            domain: "127.0.0.1",
            username: "1000",
            password: "1000",
            registrarUri: "sip:127.0.0.1",
            agentId: "agent1");

        Console.WriteLine("Registration result: " + regStatus.Message);


         var regStatus2 = await mgr.AddAccount(
            accountId: "1001",
            domain: "127.0.0.1",
            username: "1001",
            password: "1001",
            registrarUri: "sip:127.0.0.1",
            agentId: "agent2");

        Console.WriteLine("Registration result: " + regStatus2.Message);


        if (regStatus.Success)
        {
             await Task.Delay(5000);
            // Make a call using the registered account.
            mgr.MakeCall("1000", "sip:1001@127.0.0.1");

            // Wait a short while (simulate call duration) then hang up.
        
            // Here, you might look up the actual call ID; for illustration, assume a call with ID 1.
          //  mgr.HangupCall(0);
        }
       // Console.ReadKey();
   await Task.Delay(100000);
//         // Remove the account.
//         mgr.RemoveAccount("1000");
//    mgr.RemoveAccount("1001");
        // Wait a moment before shutting down.
    
    }
}
