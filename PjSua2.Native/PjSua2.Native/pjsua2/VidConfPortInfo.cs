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

public class VidConfPortInfo : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal VidConfPortInfo(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(VidConfPortInfo obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(VidConfPortInfo obj) {
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

  ~VidConfPortInfo() {
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
          pjsua2PINVOKE.delete_VidConfPortInfo(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int portId {
    set {
      pjsua2PINVOKE.VidConfPortInfo_portId_set(swigCPtr, value);
    } 
    get {
      int ret = pjsua2PINVOKE.VidConfPortInfo_portId_get(swigCPtr);
      return ret;
    } 
  }

  public string name {
    set {
      pjsua2PINVOKE.VidConfPortInfo_name_set(swigCPtr, value);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    } 
    get {
      string ret = pjsua2PINVOKE.VidConfPortInfo_name_get(swigCPtr);
      if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
      return ret;
    } 
  }

  public MediaFormatVideo format {
    set {
      pjsua2PINVOKE.VidConfPortInfo_format_set(swigCPtr, MediaFormatVideo.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VidConfPortInfo_format_get(swigCPtr);
      MediaFormatVideo ret = (cPtr == global::System.IntPtr.Zero) ? null : new MediaFormatVideo(cPtr, false);
      return ret;
    } 
  }

  public IntVector listeners {
    set {
      pjsua2PINVOKE.VidConfPortInfo_listeners_set(swigCPtr, IntVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VidConfPortInfo_listeners_get(swigCPtr);
      IntVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new IntVector(cPtr, false);
      return ret;
    } 
  }

  public IntVector transmitters {
    set {
      pjsua2PINVOKE.VidConfPortInfo_transmitters_set(swigCPtr, IntVector.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = pjsua2PINVOKE.VidConfPortInfo_transmitters_get(swigCPtr);
      IntVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new IntVector(cPtr, false);
      return ret;
    } 
  }

  public VidConfPortInfo() : this(pjsua2PINVOKE.new_VidConfPortInfo(), true) {
  }

}

}
