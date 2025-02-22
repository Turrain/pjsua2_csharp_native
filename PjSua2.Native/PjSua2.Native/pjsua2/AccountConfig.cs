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

public class AccountConfig : PersistentObject {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal AccountConfig(global::System.IntPtr cPtr, bool cMemoryOwn) : base(pjsua2PINVOKE.AccountConfig_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AccountConfig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(AccountConfig obj) {
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

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          pjsua2PINVOKE.delete_AccountConfig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public int priority {
    set {
      pjsua2PINVOKE.AccountConfig_priority_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.AccountConfig_priority_get(swigCPtr);
      return ret;
    } 
  }

  public string idUri {
    set {
      pjsua2PINVOKE.AccountConfig_idUri_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountConfig_idUri_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public AccountRegConfig regConfig {
    set {
      pjsua2PINVOKE.AccountConfig_regConfig_set(swigCPtr, AccountRegConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_regConfig_get(swigCPtr);
      AccountRegConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountRegConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountSipConfig sipConfig {
    set {
      pjsua2PINVOKE.AccountConfig_sipConfig_set(swigCPtr, AccountSipConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_sipConfig_get(swigCPtr);
      AccountSipConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountSipConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountCallConfig callConfig {
    set {
      pjsua2PINVOKE.AccountConfig_callConfig_set(swigCPtr, AccountCallConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_callConfig_get(swigCPtr);
      AccountCallConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountCallConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountPresConfig presConfig {
    set {
      pjsua2PINVOKE.AccountConfig_presConfig_set(swigCPtr, AccountPresConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_presConfig_get(swigCPtr);
      AccountPresConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountPresConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountMwiConfig mwiConfig {
    set {
      pjsua2PINVOKE.AccountConfig_mwiConfig_set(swigCPtr, AccountMwiConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_mwiConfig_get(swigCPtr);
      AccountMwiConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountMwiConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountNatConfig natConfig {
    set {
      pjsua2PINVOKE.AccountConfig_natConfig_set(swigCPtr, AccountNatConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_natConfig_get(swigCPtr);
      AccountNatConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountNatConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountMediaConfig mediaConfig {
    set {
      pjsua2PINVOKE.AccountConfig_mediaConfig_set(swigCPtr, AccountMediaConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_mediaConfig_get(swigCPtr);
      AccountMediaConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountMediaConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountVideoConfig videoConfig {
    set {
      pjsua2PINVOKE.AccountConfig_videoConfig_set(swigCPtr, AccountVideoConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_videoConfig_get(swigCPtr);
      AccountVideoConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountVideoConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountIpChangeConfig ipChangeConfig {
    set {
      pjsua2PINVOKE.AccountConfig_ipChangeConfig_set(swigCPtr, AccountIpChangeConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountConfig_ipChangeConfig_get(swigCPtr);
      AccountIpChangeConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AccountIpChangeConfig(cPtr, false);
      return ret;
    } 
  }

  public AccountConfig() : this(pjsua2PINVOKE.new_AccountConfig(), true) {
  }

  public override void readObject(ContainerNode node) {
    pjsua2PINVOKE.AccountConfig_readObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public override void writeObject(ContainerNode node) {
    pjsua2PINVOKE.AccountConfig_writeObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

}

}
