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

public class AccountSipConfig : PersistentObject {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal AccountSipConfig(global::System.IntPtr cPtr, bool cMemoryOwn) : base(pjsua2PINVOKE.AccountSipConfig_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AccountSipConfig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          pjsua2PINVOKE.delete_AccountSipConfig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public AuthCredInfoVector authCreds {
    set {
      pjsua2PINVOKE.AccountSipConfig_authCreds_set(swigCPtr, AuthCredInfoVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountSipConfig_authCreds_get(swigCPtr);
      AuthCredInfoVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new AuthCredInfoVector(cPtr, false);
      return ret;
    } 
  }

  public StringVector proxies {
    set {
      pjsua2PINVOKE.AccountSipConfig_proxies_set(swigCPtr, StringVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.AccountSipConfig_proxies_get(swigCPtr);
      StringVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new StringVector(cPtr, false);
      return ret;
    } 
  }

  public string contactForced {
    set {
      pjsua2PINVOKE.AccountSipConfig_contactForced_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountSipConfig_contactForced_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string contactParams {
    set {
      pjsua2PINVOKE.AccountSipConfig_contactParams_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountSipConfig_contactParams_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string contactUriParams {
    set {
      pjsua2PINVOKE.AccountSipConfig_contactUriParams_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountSipConfig_contactUriParams_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public bool authInitialEmpty {
    set {
      pjsua2PINVOKE.AccountSipConfig_authInitialEmpty_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountSipConfig_authInitialEmpty_get(swigCPtr);
      return ret;
    } 
  }

  public string authInitialAlgorithm {
    set {
      pjsua2PINVOKE.AccountSipConfig_authInitialAlgorithm_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.AccountSipConfig_authInitialAlgorithm_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public int transportId {
    set {
      pjsua2PINVOKE.AccountSipConfig_transportId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.AccountSipConfig_transportId_get(swigCPtr);
      return ret;
    } 
  }

  public pjsua_ipv6_use ipv6Use {
    set {
      pjsua2PINVOKE.AccountSipConfig_ipv6Use_set(swigCPtr, (int)value);
    } 
    get {
      pjsua_ipv6_use ret = (pjsua_ipv6_use)pjsua2PINVOKE.AccountSipConfig_ipv6Use_get(swigCPtr);
      return ret;
    } 
  }

  public bool useSharedAuth {
    set {
      pjsua2PINVOKE.AccountSipConfig_useSharedAuth_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.AccountSipConfig_useSharedAuth_get(swigCPtr);
      return ret;
    } 
  }

  public override void readObject(ContainerNode node) {
    pjsua2PINVOKE.AccountSipConfig_readObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public override void writeObject(ContainerNode node) {
    pjsua2PINVOKE.AccountSipConfig_writeObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public AccountSipConfig() : this(pjsua2PINVOKE.new_AccountSipConfig(), true) {
  }

}

}
