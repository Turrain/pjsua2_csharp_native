using System;
using PjSua2.Native;
namespace PjSua2.Lx
{
public struct RegistrationStatus
{
    public bool Success;
    public string Message;
    public int StatusCode;
}

public class AgentManager
{
    private static readonly Lazy<AgentManager> _instance = 
        new Lazy<AgentManager>(() => new AgentManager());
        
    public static AgentManager Instance => _instance.Value;
    
    public Agent GetAgent(string agentId)
    {
        // Implementation
        return new Agent();
    }
}

public class Agent { }
}