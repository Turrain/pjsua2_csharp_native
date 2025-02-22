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

public class OnCallMediaTransportStateParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal OnCallMediaTransportStateParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(OnCallMediaTransportStateParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(OnCallMediaTransportStateParam obj) {
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

  ~OnCallMediaTransportStateParam() {
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
          pjsua2PINVOKE.delete_OnCallMediaTransportStateParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public uint medIdx {
    set {
      pjsua2PINVOKE.OnCallMediaTransportStateParam_medIdx_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.OnCallMediaTransportStateParam_medIdx_get(swigCPtr);
      return ret;
    } 
  }

  public pjsua_med_tp_st state {
    set {
      pjsua2PINVOKE.OnCallMediaTransportStateParam_state_set(swigCPtr, (int)value);
    } 
    get {
      pjsua_med_tp_st ret = (pjsua_med_tp_st)pjsua2PINVOKE.OnCallMediaTransportStateParam_state_get(swigCPtr);
      return ret;
    } 
  }

  public int status {
    set {
      pjsua2PINVOKE.OnCallMediaTransportStateParam_status_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.OnCallMediaTransportStateParam_status_get(swigCPtr);
      return ret;
    } 
  }

  public int sipErrorCode {
    set {
      pjsua2PINVOKE.OnCallMediaTransportStateParam_sipErrorCode_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.OnCallMediaTransportStateParam_sipErrorCode_get(swigCPtr);
      return ret;
    } 
  }

  public OnCallMediaTransportStateParam() : this(pjsua2PINVOKE.new_OnCallMediaTransportStateParam(), true) {
  }

}

}
