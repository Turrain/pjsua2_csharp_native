using System;
using PjSua2.Native;
using PjSua2.Native.pjsua2;
namespace PjSua2.Lx
{

public class Account : Native.pjsua2.Account
{
    public TaskCompletionSource<TaskStatus> RegTcs { get; set; }

    public string AgentId { get; set; }
    public Agent _agent = new();
    private AgentManager _agentManager = AgentManager.Instance;

   public Account(TaskCompletionSource<TaskStatus> tcs)
        {
            RegTcs = tcs;
        }


    public override void onRegState(OnRegStateParam prm)
    {
          base.onRegState(prm);
          
         bool success = prm.code == pjsip_status_code.PJSIP_SC_OK;
            TaskStatus status = new TaskStatus
            {
                Success = success,
                Message = success
                    ? "Registration successful"
                    : $"Registration failed with code: {prm.code}",
                StatusCode = (int)prm.code
            };

            // Ensure the registration result is set only once.
            if (!RegTcs.Task.IsCompleted)
            {
                RegTcs.SetResult(status);
            }
    }

    public override void onIncomingCall(OnIncomingCallParam iprm)
    {
        // Handle incoming call
        base.onIncomingCall(iprm);
        Call call = new Call(this, iprm.callId);
        CallOpParam callOpParam = new CallOpParam();
        callOpParam.statusCode = pjsip_status_code.PJSIP_SC_OK;
        call.CallDirection = Call.Direction.Incoming;
        call.answer(callOpParam);
    }

    ~Account()
    {
        // Cleanup if needed
    }
}

}