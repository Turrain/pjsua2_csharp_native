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

public class EpConfig : PersistentObject {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal EpConfig(global::System.IntPtr cPtr, bool cMemoryOwn) : base(pjsua2PINVOKE.EpConfig_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(EpConfig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(EpConfig obj) {
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
          pjsua2PINVOKE.delete_EpConfig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public UaConfig uaConfig {
    set {
      pjsua2PINVOKE.EpConfig_uaConfig_set(swigCPtr, UaConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.EpConfig_uaConfig_get(swigCPtr);
      UaConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new UaConfig(cPtr, false);
      return ret;
    } 
  }

  public LogConfig logConfig {
    set {
      pjsua2PINVOKE.EpConfig_logConfig_set(swigCPtr, LogConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.EpConfig_logConfig_get(swigCPtr);
      LogConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new LogConfig(cPtr, false);
      return ret;
    } 
  }

  public MediaConfig medConfig {
    set {
      pjsua2PINVOKE.EpConfig_medConfig_set(swigCPtr, MediaConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.EpConfig_medConfig_get(swigCPtr);
      MediaConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new MediaConfig(cPtr, false);
      return ret;
    } 
  }

  public override void readObject(ContainerNode node) {
    pjsua2PINVOKE.EpConfig_readObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public override void writeObject(ContainerNode node) {
    pjsua2PINVOKE.EpConfig_writeObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public EpConfig() : this(pjsua2PINVOKE.new_EpConfig(), true) {
  }

}

}
