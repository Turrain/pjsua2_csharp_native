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

public class AccountInfo : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal AccountInfo(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AccountInfo obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(AccountInfo obj) {
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

  ~AccountInfo() {
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
          pjsua2PINVOKE.delete_AccountInfo(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int id {
    set {
      pjsua2PINVOKE.AccountInfo_id_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.AccountInfo_id_get(swigCPtr);
      return ret;
    } 
  }

  public bool isDefault {
    set {
      pjsua2PINVOKE.AccountInfo_isDefault_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountInfo_isDefault_get(swigCPtr);
      return ret;
    } 
  }

  public string uri {
    set {
      pjsua2PINVOKE.AccountInfo_uri_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountInfo_uri_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public bool regIsConfigured {
    set {
      pjsua2PINVOKE.AccountInfo_regIsConfigured_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountInfo_regIsConfigured_get(swigCPtr);
      return ret;
    } 
  }

  public bool regIsActive {
    set {
      pjsua2PINVOKE.AccountInfo_regIsActive_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountInfo_regIsActive_get(swigCPtr);
      return ret;
    } 
  }

  public uint regExpiresSec {
    set {
      pjsua2PINVOKE.AccountInfo_regExpiresSec_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.AccountInfo_regExpiresSec_get(swigCPtr);
      return ret;
    } 
  }

  public pjsip_status_code regStatus {
    set {
      pjsua2PINVOKE.AccountInfo_regStatus_set(swigCPtr, (int)value);
    } 
    get {
      pjsip_status_code ret = (pjsip_status_code)pjsua2PINVOKE.AccountInfo_regStatus_get(swigCPtr);
      return ret;
    } 
  }

  public string regStatusText {
    set {
      pjsua2PINVOKE.AccountInfo_regStatusText_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountInfo_regStatusText_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public int regLastErr {
    set {
      pjsua2PINVOKE.AccountInfo_regLastErr_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.AccountInfo_regLastErr_get(swigCPtr);
      return ret;
    } 
  }

  public bool onlineStatus {
    set {
      pjsua2PINVOKE.AccountInfo_onlineStatus_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountInfo_onlineStatus_get(swigCPtr);
      return ret;
    } 
  }

  public string onlineStatusText {
    set {
      pjsua2PINVOKE.AccountInfo_onlineStatusText_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountInfo_onlineStatusText_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public AccountInfo() : this(pjsua2PINVOKE.new_AccountInfo(), true) {
  }

}

}
