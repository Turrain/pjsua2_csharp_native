//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace PjSua2.Native.pjsua2 {

public class OnIpChangeProgressParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal OnIpChangeProgressParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(OnIpChangeProgressParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~OnIpChangeProgressParam() {
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
          pjsua2PINVOKE.delete_OnIpChangeProgressParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public pjsua_ip_change_op op {
    set {
      pjsua2PINVOKE.OnIpChangeProgressParam_op_set(swigCPtr, (int)value);
    } 
    get {
      pjsua_ip_change_op ret = (pjsua_ip_change_op)pjsua2PINVOKE.OnIpChangeProgressParam_op_get(swigCPtr);
      return ret;
    } 
  }

  public int status {
    set {
      pjsua2PINVOKE.OnIpChangeProgressParam_status_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.OnIpChangeProgressParam_status_get(swigCPtr);
      return ret;
    } 
  }

  public int transportId {
    set {
      pjsua2PINVOKE.OnIpChangeProgressParam_transportId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.OnIpChangeProgressParam_transportId_get(swigCPtr);
      return ret;
    } 
  }

  public int accId {
    set {
      pjsua2PINVOKE.OnIpChangeProgressParam_accId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.OnIpChangeProgressParam_accId_get(swigCPtr);
      return ret;
    } 
  }

  public int callId {
    set {
      pjsua2PINVOKE.OnIpChangeProgressParam_callId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.OnIpChangeProgressParam_callId_get(swigCPtr);
      return ret;
    } 
  }

  public RegProgressParam regInfo {
    set {
      pjsua2PINVOKE.OnIpChangeProgressParam_regInfo_set(swigCPtr, RegProgressParam.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.OnIpChangeProgressParam_regInfo_get(swigCPtr);
      RegProgressParam ret = (cPtr == global::System.IntPtr.Zero) ? null : new RegProgressParam(cPtr, false);
      return ret;
    } 
  }

  public OnIpChangeProgressParam() : this(pjsua2PINVOKE.new_OnIpChangeProgressParam(), true) {
  }

}

}
