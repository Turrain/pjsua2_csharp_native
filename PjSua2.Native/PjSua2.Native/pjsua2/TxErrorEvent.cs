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

public class TxErrorEvent : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal TxErrorEvent(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(TxErrorEvent obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~TxErrorEvent() {
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
          pjsua2PINVOKE.delete_TxErrorEvent(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public SipTxData tdata {
    set {
      pjsua2PINVOKE.TxErrorEvent_tdata_set(swigCPtr, SipTxData.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TxErrorEvent_tdata_get(swigCPtr);
      SipTxData ret = (cPtr == global::System.IntPtr.Zero) ? null : new SipTxData(cPtr, false);
      return ret;
    } 
  }

  public SipTransaction tsx {
    set {
      pjsua2PINVOKE.TxErrorEvent_tsx_set(swigCPtr, SipTransaction.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.TxErrorEvent_tsx_get(swigCPtr);
      SipTransaction ret = (cPtr == global::System.IntPtr.Zero) ? null : new SipTransaction(cPtr, false);
      return ret;
    } 
  }

  public TxErrorEvent() : this(pjsua2PINVOKE.new_TxErrorEvent(), true) {
  }

}

}
