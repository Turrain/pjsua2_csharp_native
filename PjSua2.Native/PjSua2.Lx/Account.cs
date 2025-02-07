using System;
using PjSua2.Native;
using PjSua2.Native.pjsua2;
namespace PjSua2.Lx
{

public class Account : Native.pjsua2.Account
{
    public delegate void OnRegStateDelegate(bool isSuccess, int status);
    public OnRegStateDelegate RegStateCallback;

    private string _agentId;
    private Agent _agent;
    private AgentManager _agentManager = AgentManager.Instance;

    public Account()
    {
    }

    public void SetAgent(string agentId)
    {
        _agentId = agentId;
        _agent = _agentManager.GetAgent(agentId);
    }

    public Agent GetAgent()
    {
        return _agent;
    }

    public void RegisterRegStateCallback(OnRegStateDelegate callback)
    {
        RegStateCallback = callback;
    }

    public override void onRegState(OnRegStateParam prm)
    {
        var info = getInfo();
        
        bool isSuccess = (int)prm.code / 100 == 2;
        RegStateCallback?.Invoke(isSuccess, (int)prm.code);
    }

    public override void onIncomingCall(OnIncomingCallParam iprm)
    {
        // Handle incoming call
        base.onIncomingCall(iprm);
    }

    ~Account()
    {
        // Cleanup if needed
    }
}

}