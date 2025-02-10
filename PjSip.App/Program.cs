
using System.Text;
using PjSua2.Lx;
using PjSua2.Lx.AudioStream;
using PjSua2.Native;
using PjSua2.Native.pjsua2;
//GenHttp
class Program
{
    static async Task Main(string[] args)
    {


        // --- Example usage of AuralisClient ---
        Console.WriteLine("\n=== AuralisClient Example ===");
        string auralisUri = "ws://37.151.89.206:8766"; // Replace with your actual URI
        var auralisClient = new AuralisClient(auralisUri);

        List<byte[]> bytes = [];
        auralisClient.OnBinaryMessage += data =>
        {
            bytes.Add(data);
            Console.WriteLine("Auralis Received Binary: " + BitConverter.ToString(data));
        };
        auralisClient.OnError += ex =>
        {
            Console.WriteLine("Auralis Error: " + ex.Message);
        };

        try
        {
            // Connect the client
            await auralisClient.ConnectAsync();
            Console.WriteLine("AuralisClient connected.");

            // Send a JSON command (which in AuralisClient sends as text)

            var input = new
            {
                input = "test",
                voice = "default",
                stream = true,
                temperature = 0.5
            };
            await auralisClient.SendCommandAsync(input);
            Console.WriteLine("AuralisClient sent JSON command.");

            // Wait for some binary messages (adjust as needed)
            await Task.Delay(5000);

            // Disconnect when done
            await auralisClient.DisconnectAsync();
            Console.WriteLine("AuralisClient disconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in AuralisClient: " + ex.Message);
        }
        finally
        {
            await auralisClient.DisposeAsync();
        }
        await Task.Delay(5000);
        Console.WriteLine("=== WhisperClient Example ===");
        string whisperUri = "ws://37.151.89.206:8765"; // Replace with your actual URI
        var whisperClient = new WhisperClient(whisperUri);


        whisperClient.OnJsonMessage += json =>
        {
           
            Console.WriteLine("Whisper Received JSON: " + json.ToString());
            Console.WriteLine(json.GetProperty("text"));
        };
        whisperClient.OnError += ex =>
        {
            Console.WriteLine("Whisper Error: " + ex.Message);
        };

        try
        {
            // Connect the client
            await whisperClient.ConnectAsync();
            Console.WriteLine("WhisperClient connected.");

          

            await whisperClient.SendAudioAsync(bytes[0]);
            Console.WriteLine("WhisperClient sent JSON as binary.");

            // Optionally, send some raw audio binary data
            byte[] dummyAudioData = Encoding.UTF8.GetBytes("Dummy audio data");
            await whisperClient.SendAudioAsync(dummyAudioData);
            Console.WriteLine("WhisperClient sent audio binary data.");

            // Wait for some messages (adjust as needed)
            await Task.Delay(5000);

            // Disconnect when done
            await whisperClient.DisconnectAsync();
            Console.WriteLine("WhisperClient disconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in WhisperClient: " + ex.Message);
        }
        finally
        {
            await whisperClient.DisposeAsync();
        }



        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();


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
