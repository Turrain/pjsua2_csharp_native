//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.2.0
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace PjSua2.Native.pjsua2 {

public class CallInfo : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal CallInfo(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(CallInfo obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(CallInfo obj) {
    if (obj != null) {
      if (!obj.swigCMemOwn)
        throw new global::System.ApplicationException("Cannot release ownership as memory is not owned");
      global::System.Runtime.InteropServices.HandleRef ptr = obj.swigCPtr;
      obj.swigCMemOwn = false;
      obj.Dispose();
      return ptr;
    } else {
      return new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
    }
  }

  ~CallInfo() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          pjsua2PINVOKE.delete_CallInfo(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int id {
    set {
      pjsua2PINVOKE.CallInfo_id_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.CallInfo_id_get(swigCPtr);
      return ret;
    } 
  }

  public pjsip_role_e role {
    set {
      pjsua2PINVOKE.CallInfo_role_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_role_e ret = (pjsip_role_e)pjsua2PINVOKE.CallInfo_role_get(swigCPtr);
      return ret;
    } 
  }

  public int accId {
    set {
      pjsua2PINVOKE.CallInfo_accId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.CallInfo_accId_get(swigCPtr);
      return ret;
    } 
  }

  public string localUri {
    set {
      pjsua2PINVOKE.CallInfo_localUri_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_localUri_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string localContact {
    set {
      pjsua2PINVOKE.CallInfo_localContact_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_localContact_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string remoteUri {
    set {
      pjsua2PINVOKE.CallInfo_remoteUri_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_remoteUri_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string remoteContact {
    set {
      pjsua2PINVOKE.CallInfo_remoteContact_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_remoteContact_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string callIdString {
    set {
      pjsua2PINVOKE.CallInfo_callIdString_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_callIdString_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public CallSetting setting {
    set {
      pjsua2PINVOKE.CallInfo_setting_set(swigCPtr, CallSetting.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.CallInfo_setting_get(swigCPtr);
      CallSetting ret = (cPtr == global::System.IntPtr.Zero) ? null : new CallSetting(cPtr, false);
      return ret;
    } 
  }

  public pjsip_inv_state state {
    set {
      pjsua2PINVOKE.CallInfo_state_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_inv_state ret = (pjsip_inv_state)pjsua2PINVOKE.CallInfo_state_get(swigCPtr);
      return ret;
    } 
  }

  public string stateText {
    set {
      pjsua2PINVOKE.CallInfo_stateText_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_stateText_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public pjsip_status_code lastStatusCode {
    set {
      pjsua2PINVOKE.CallInfo_lastStatusCode_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_status_code ret = (pjsip_status_code)pjsua2PINVOKE.CallInfo_lastStatusCode_get(swigCPtr);
      return ret;
    } 
  }

  public string lastReason {
    set {
      pjsua2PINVOKE.CallInfo_lastReason_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.CallInfo_lastReason_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public CallMediaInfoVector media {
    set {
      pjsua2PINVOKE.CallInfo_media_set(swigCPtr, CallMediaInfoVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.CallInfo_media_get(swigCPtr);
      CallMediaInfoVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new CallMediaInfoVector(cPtr, false);
      return ret;
    } 
  }

  public CallMediaInfoVector provMedia {
    set {
      pjsua2PINVOKE.CallInfo_provMedia_set(swigCPtr, CallMediaInfoVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.CallInfo_provMedia_get(swigCPtr);
      CallMediaInfoVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new CallMediaInfoVector(cPtr, false);
      return ret;
    } 
  }

  public TimeVal connectDuration {
    set {
      pjsua2PINVOKE.CallInfo_connectDuration_set(swigCPtr, TimeVal.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.CallInfo_connectDuration_get(swigCPtr);
      TimeVal ret = (cPtr == global::System.IntPtr.Zero) ? null : new TimeVal(cPtr, false);
      return ret;
    } 
  }

  public TimeVal totalDuration {
    set {
      pjsua2PINVOKE.CallInfo_totalDuration_set(swigCPtr, TimeVal.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.CallInfo_totalDuration_get(swigCPtr);
      TimeVal ret = (cPtr == global::System.IntPtr.Zero) ? null : new TimeVal(cPtr, false);
      return ret;
    } 
  }

  public bool remOfferer {
    set {
      pjsua2PINVOKE.CallInfo_remOfferer_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.CallInfo_remOfferer_get(swigCPtr);
      return ret;
    } 
  }

  public uint remAudioCount {
    set {
      pjsua2PINVOKE.CallInfo_remAudioCount_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.CallInfo_remAudioCount_get(swigCPtr);
      return ret;
    } 
  }

  public uint remVideoCount {
    set {
      pjsua2PINVOKE.CallInfo_remVideoCount_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.CallInfo_remVideoCount_get(swigCPtr);
      return ret;
    } 
  }

  public CallInfo() : this(pjsua2PINVOKE.new_CallInfo(), true) {
  }

}

}
