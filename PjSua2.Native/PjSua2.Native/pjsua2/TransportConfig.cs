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

public class TransportConfig : PersistentObject {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal TransportConfig(global::System.IntPtr cPtr, bool cMemoryOwn) : base(pjsua2PINVOKE.TransportConfig_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(TransportConfig obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(TransportConfig obj) {
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
          pjsua2PINVOKE.delete_TransportConfig(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public uint port {
    set {
      pjsua2PINVOKE.TransportConfig_port_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.TransportConfig_port_get(swigCPtr);
      return ret;
    } 
  }

  public uint portRange {
    set {
      pjsua2PINVOKE.TransportConfig_portRange_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.TransportConfig_portRange_get(swigCPtr);
      return ret;
    } 
  }

  public bool randomizePort {
    set {
      pjsua2PINVOKE.TransportConfig_randomizePort_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.TransportConfig_randomizePort_get(swigCPtr);
      return ret;
    } 
  }

  public string publicAddress {
    set {
      pjsua2PINVOKE.TransportConfig_publicAddress_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.TransportConfig_publicAddress_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public string boundAddress {
    set {
      pjsua2PINVOKE.TransportConfig_boundAddress_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.TransportConfig_boundAddress_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public TlsConfig tlsConfig {
    set {
      pjsua2PINVOKE.TransportConfig_tlsConfig_set(swigCPtr, TlsConfig.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TransportConfig_tlsConfig_get(swigCPtr);
      TlsConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new TlsConfig(cPtr, false);
      return ret;
    } 
  }

  public pj_qos_type qosType {
    set {
      pjsua2PINVOKE.TransportConfig_qosType_set(swigCPtr, (int)value);
    } 
    get {
      pj_qos_type ret = (pj_qos_type)pjsua2PINVOKE.TransportConfig_qosType_get(swigCPtr);
      return ret;
    } 
  }

  public pj_qos_params qosParams {
    set {
      pjsua2PINVOKE.TransportConfig_qosParams_set(swigCPtr, pj_qos_params.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TransportConfig_qosParams_get(swigCPtr);
      pj_qos_params ret = (cPtr == global::System.IntPtr.Zero) ? null : new pj_qos_params(cPtr, false);
      return ret;
    } 
  }

  public SockOptParams sockOptParams {
    set {
      pjsua2PINVOKE.TransportConfig_sockOptParams_set(swigCPtr, SockOptParams.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TransportConfig_sockOptParams_get(swigCPtr);
      SockOptParams ret = (cPtr == global::System.IntPtr.Zero) ? null : new SockOptParams(cPtr, false);
      return ret;
    } 
  }

  public TransportConfig() : this(pjsua2PINVOKE.new_TransportConfig(), true) {
  }

  public override void readObject(ContainerNode node) {
    pjsua2PINVOKE.TransportConfig_readObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public override void writeObject(ContainerNode node) {
    pjsua2PINVOKE.TransportConfig_writeObject(swigCPtr, ContainerNode.getCPtr(node));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

}

}
