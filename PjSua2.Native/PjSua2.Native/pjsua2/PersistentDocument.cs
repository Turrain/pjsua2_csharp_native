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

public class PersistentDocument : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal PersistentDocument(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(PersistentDocument obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(PersistentDocument obj) {
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

  ~PersistentDocument() {
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
          pjsua2PINVOKE.delete_PersistentDocument(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public virtual void loadFile(string filename) {
    pjsua2PINVOKE.PersistentDocument_loadFile(swigCPtr, filename);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void loadString(string input) {
    pjsua2PINVOKE.PersistentDocument_loadString(swigCPtr, input);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual void saveFile(string filename) {
    pjsua2PINVOKE.PersistentDocument_saveFile(swigCPtr, filename);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public virtual string saveString() {
    string ret = pjsua2PINVOKE.PersistentDocument_saveString(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public virtual ContainerNode getRootContainer() {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_getRootContainer(swigCPtr), false);
    return ret;
  }

  public bool hasUnread() {
    bool ret = pjsua2PINVOKE.PersistentDocument_hasUnread(swigCPtr);
    return ret;
  }

  public string unreadName() {
    string ret = pjsua2PINVOKE.PersistentDocument_unreadName(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int readInt(string name) {
    int ret = pjsua2PINVOKE.PersistentDocument_readInt__SWIG_0(swigCPtr, name);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int readInt() {
    int ret = pjsua2PINVOKE.PersistentDocument_readInt__SWIG_1(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float readNumber(string name) {
    float ret = pjsua2PINVOKE.PersistentDocument_readNumber__SWIG_0(swigCPtr, name);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public float readNumber() {
    float ret = pjsua2PINVOKE.PersistentDocument_readNumber__SWIG_1(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool readBool(string name) {
    bool ret = pjsua2PINVOKE.PersistentDocument_readBool__SWIG_0(swigCPtr, name);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool readBool() {
    bool ret = pjsua2PINVOKE.PersistentDocument_readBool__SWIG_1(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string readString(string name) {
    string ret = pjsua2PINVOKE.PersistentDocument_readString__SWIG_0(swigCPtr, name);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string readString() {
    string ret = pjsua2PINVOKE.PersistentDocument_readString__SWIG_1(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public StringVector readStringVector(string name) {
    StringVector ret = new StringVector(pjsua2PINVOKE.PersistentDocument_readStringVector__SWIG_0(swigCPtr, name), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public StringVector readStringVector() {
    StringVector ret = new StringVector(pjsua2PINVOKE.PersistentDocument_readStringVector__SWIG_1(swigCPtr), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void readObject(PersistentObject obj) {
    pjsua2PINVOKE.PersistentDocument_readObject(swigCPtr, PersistentObject.getCPtr(obj));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public ContainerNode readContainer(string name) {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_readContainer__SWIG_0(swigCPtr, name), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public ContainerNode readContainer() {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_readContainer__SWIG_1(swigCPtr), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public ContainerNode readArray(string name) {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_readArray__SWIG_0(swigCPtr, name), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public ContainerNode readArray() {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_readArray__SWIG_1(swigCPtr), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void writeNumber(string name, float num) {
    pjsua2PINVOKE.PersistentDocument_writeNumber(swigCPtr, name, num);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void writeInt(string name, int num) {
    pjsua2PINVOKE.PersistentDocument_writeInt(swigCPtr, name, num);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void writeBool(string name, bool value) {
    pjsua2PINVOKE.PersistentDocument_writeBool(swigCPtr, name, value);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void writeString(string name, string value) {
    pjsua2PINVOKE.PersistentDocument_writeString(swigCPtr, name, value);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void writeStringVector(string name, StringVector arr) {
    pjsua2PINVOKE.PersistentDocument_writeStringVector(swigCPtr, name, StringVector.getCPtr(arr));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void writeObject(PersistentObject obj) {
    pjsua2PINVOKE.PersistentDocument_writeObject(swigCPtr, PersistentObject.getCPtr(obj));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public ContainerNode writeNewContainer(string name) {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_writeNewContainer(swigCPtr, name), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public ContainerNode writeNewArray(string name) {
    ContainerNode ret = new ContainerNode(pjsua2PINVOKE.PersistentDocument_writeNewArray(swigCPtr, name), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
