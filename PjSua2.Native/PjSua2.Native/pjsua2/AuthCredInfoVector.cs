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

public class AuthCredInfoVector : global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IEnumerable<AuthCredInfo>
 {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal AuthCredInfoVector(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AuthCredInfoVector obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(AuthCredInfoVector obj) {
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

  ~AuthCredInfoVector() {
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
          pjsua2PINVOKE.delete_AuthCredInfoVector(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public AuthCredInfoVector(global::System.Collections.IEnumerable c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (AuthCredInfo element in c) {
      this.Add(element);
    }
  }

  public AuthCredInfoVector(global::System.Collections.Generic.IEnumerable<AuthCredInfo> c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (AuthCredInfo element in c) {
      this.Add(element);
    }
  }

  public bool IsFixedSize {
    get {
      return false;
    }
  }

  public bool IsReadOnly {
    get {
      return false;
    }
  }

  public AuthCredInfo this[int index]  {
    get {
      return getitem(index);
    }
    set {
      setitem(index, value);
    }
  }

  public int Capacity {
    get {
      return (int)capacity();
    }
    set {
      if (value < 0 || (uint)value < size())
        throw new global::System.ArgumentOutOfRangeException("Capacity");
      reserve((uint)value);
    }
  }

  public bool IsEmpty {
    get {
      return empty();
    }
  }

  public int Count {
    get {
      return (int)size();
    }
  }

  public bool IsSynchronized {
    get {
      return false;
    }
  }

  public void CopyTo(AuthCredInfo[] array)
  {
    CopyTo(0, array, 0, this.Count);
  }

  public void CopyTo(AuthCredInfo[] array, int arrayIndex)
  {
    CopyTo(0, array, arrayIndex, this.Count);
  }

  public void CopyTo(int index, AuthCredInfo[] array, int arrayIndex, int count)
  {
    if (array == null)
      throw new global::System.ArgumentNullException("array");
    if (index < 0)
      throw new global::System.ArgumentOutOfRangeException("index", "Value is less than zero");
    if (arrayIndex < 0)
      throw new global::System.ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
    if (count < 0)
      throw new global::System.ArgumentOutOfRangeException("count", "Value is less than zero");
    if (array.Rank > 1)
      throw new global::System.ArgumentException("Multi dimensional array.", "array");
    if (index+count > this.Count || arrayIndex+count > array.Length)
      throw new global::System.ArgumentException("Number of elements to copy is too large.");
    for (int i=0; i<count; i++)
      array.SetValue(getitemcopy(index+i), arrayIndex+i);
  }

  public AuthCredInfo[] ToArray() {
    AuthCredInfo[] array = new AuthCredInfo[this.Count];
    this.CopyTo(array);
    return array;
  }

  global::System.Collections.Generic.IEnumerator<AuthCredInfo> global::System.Collections.Generic.IEnumerable<AuthCredInfo>.GetEnumerator() {
    return new AuthCredInfoVectorEnumerator(this);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
    return new AuthCredInfoVectorEnumerator(this);
  }

  public AuthCredInfoVectorEnumerator GetEnumerator() {
    return new AuthCredInfoVectorEnumerator(this);
  }

  // Type-safe enumerator
  /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
  /// whenever the collection is modified. This has been done for changes in the size of the
  /// collection but not when one of the elements of the collection is modified as it is a bit
  /// tricky to detect unmanaged code that modifies the collection under our feet.
  public sealed class AuthCredInfoVectorEnumerator : global::System.Collections.IEnumerator
    , global::System.Collections.Generic.IEnumerator<AuthCredInfo>
  {
    private AuthCredInfoVector collectionRef;
    private int currentIndex;
    private object currentObject;
    private int currentSize;

    public AuthCredInfoVectorEnumerator(AuthCredInfoVector collection) {
      collectionRef = collection;
      currentIndex = -1;
      currentObject = null;
      currentSize = collectionRef.Count;
    }

    // Type-safe iterator Current
    public AuthCredInfo Current {
      get {
        if (currentIndex == -1)
          throw new global::System.InvalidOperationException("Enumeration not started.");
        if (currentIndex > currentSize - 1)
          throw new global::System.InvalidOperationException("Enumeration finished.");
        if (currentObject == null)
          throw new global::System.InvalidOperationException("Collection modified.");
        return (AuthCredInfo)currentObject;
      }
    }

    // Type-unsafe IEnumerator.Current
    object global::System.Collections.IEnumerator.Current {
      get {
        return Current;
      }
    }

    public bool MoveNext() {
      int size = collectionRef.Count;
      bool moveOkay = (currentIndex+1 < size) && (size == currentSize);
      if (moveOkay) {
        currentIndex++;
        currentObject = collectionRef[currentIndex];
      } else {
        currentObject = null;
      }
      return moveOkay;
    }

    public void Reset() {
      currentIndex = -1;
      currentObject = null;
      if (collectionRef.Count != currentSize) {
        throw new global::System.InvalidOperationException("Collection modified.");
      }
    }

    public void Dispose() {
        currentIndex = -1;
        currentObject = null;
    }
  }

  public AuthCredInfoVector() : this(pjsua2PINVOKE.new_AuthCredInfoVector__SWIG_0(), true) {
  }

  public AuthCredInfoVector(AuthCredInfoVector other) : this(pjsua2PINVOKE.new_AuthCredInfoVector__SWIG_1(AuthCredInfoVector.getCPtr(other)), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void Clear() {
    pjsua2PINVOKE.AuthCredInfoVector_Clear(swigCPtr);
  }

  public void Add(AuthCredInfo x) {
    pjsua2PINVOKE.AuthCredInfoVector_Add(swigCPtr, AuthCredInfo.getCPtr(x));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  private uint size() {
    uint ret = pjsua2PINVOKE.AuthCredInfoVector_size(swigCPtr);
    return ret;
  }

  private bool empty() {
    bool ret = pjsua2PINVOKE.AuthCredInfoVector_empty(swigCPtr);
    return ret;
  }

  private uint capacity() {
    uint ret = pjsua2PINVOKE.AuthCredInfoVector_capacity(swigCPtr);
    return ret;
  }

  private void reserve(uint n) {
    pjsua2PINVOKE.AuthCredInfoVector_reserve(swigCPtr, n);
  }

  public AuthCredInfoVector(int capacity) : this(pjsua2PINVOKE.new_AuthCredInfoVector__SWIG_2(capacity), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  private AuthCredInfo getitemcopy(int index) {
    AuthCredInfo ret = new AuthCredInfo(pjsua2PINVOKE.AuthCredInfoVector_getitemcopy(swigCPtr, index), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private AuthCredInfo getitem(int index) {
    AuthCredInfo ret = new AuthCredInfo(pjsua2PINVOKE.AuthCredInfoVector_getitem(swigCPtr, index), false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void setitem(int index, AuthCredInfo val) {
    pjsua2PINVOKE.AuthCredInfoVector_setitem(swigCPtr, index, AuthCredInfo.getCPtr(val));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void AddRange(AuthCredInfoVector values) {
    pjsua2PINVOKE.AuthCredInfoVector_AddRange(swigCPtr, AuthCredInfoVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public AuthCredInfoVector GetRange(int index, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AuthCredInfoVector_GetRange(swigCPtr, index, count);
    AuthCredInfoVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new AuthCredInfoVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Insert(int index, AuthCredInfo x) {
    pjsua2PINVOKE.AuthCredInfoVector_Insert(swigCPtr, index, AuthCredInfo.getCPtr(x));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void InsertRange(int index, AuthCredInfoVector values) {
    pjsua2PINVOKE.AuthCredInfoVector_InsertRange(swigCPtr, index, AuthCredInfoVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAt(int index) {
    pjsua2PINVOKE.AuthCredInfoVector_RemoveAt(swigCPtr, index);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveRange(int index, int count) {
    pjsua2PINVOKE.AuthCredInfoVector_RemoveRange(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public static AuthCredInfoVector Repeat(AuthCredInfo value, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AuthCredInfoVector_Repeat(AuthCredInfo.getCPtr(value), count);
    AuthCredInfoVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new AuthCredInfoVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Reverse() {
    pjsua2PINVOKE.AuthCredInfoVector_Reverse__SWIG_0(swigCPtr);
  }

  public void Reverse(int index, int count) {
    pjsua2PINVOKE.AuthCredInfoVector_Reverse__SWIG_1(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetRange(int index, AuthCredInfoVector values) {
    pjsua2PINVOKE.AuthCredInfoVector_SetRange(swigCPtr, index, AuthCredInfoVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

}

}
