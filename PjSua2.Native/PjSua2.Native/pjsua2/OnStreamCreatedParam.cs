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

public class OnStreamCreatedParam : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal OnStreamCreatedParam(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(OnStreamCreatedParam obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(OnStreamCreatedParam obj) {
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

  ~OnStreamCreatedParam() {
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
          pjsua2PINVOKE.delete_OnStreamCreatedParam(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public System.IntPtr stream {
    set {
      pjsua2PINVOKE.OnStreamCreatedParam_stream_set(swigCPtr, value);
    } 
    get {
        System.IntPtr cPtr = pjsua2PINVOKE.OnStreamCreatedParam_stream_get(swigCPtr);
        return cPtr;
    }
  
  }

  public uint streamIdx {
    set {
      pjsua2PINVOKE.OnStreamCreatedParam_streamIdx_set(swigCPtr, value);
    } 
    get {
      uint ret = pjsua2PINVOKE.OnStreamCreatedParam_streamIdx_get(swigCPtr);
      return ret;
    } 
  }

  public bool destroyPort {
    set {
      pjsua2PINVOKE.OnStreamCreatedParam_destroyPort_set(swigCPtr, value);
    } 
    get {
      bool ret = pjsua2PINVOKE.OnStreamCreatedParam_destroyPort_get(swigCPtr);
      return ret;
    } 
  }

  public System.IntPtr pPort {
    set {
      pjsua2PINVOKE.OnStreamCreatedParam_pPort_set(swigCPtr, value);
    } 
    get {
        System.IntPtr cPtr = pjsua2PINVOKE.OnStreamCreatedParam_pPort_get(swigCPtr);
        return cPtr;
    }
  
  }

  public OnStreamCreatedParam() : this(pjsua2PINVOKE.new_OnStreamCreatedParam(), true) {
  }

}

}
