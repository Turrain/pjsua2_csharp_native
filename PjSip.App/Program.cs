﻿
using PjSua2.Native;
//GenHttp
class Program
{
    static void Main(string[] args)
    {
        try
        {
            using var pjsip = new PjSipManager();
            // Initialize PJSIP
            pjsip.Initialize();
            
            // Create SIP account
            pjsip.CreateAccount(
                username: "your_username",
                password: "your_password",
                domain: "sip.example.com"
            );

            Console.WriteLine("PJSIP initialized successfully. Press any key to exit.");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
