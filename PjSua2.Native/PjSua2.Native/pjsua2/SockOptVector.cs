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

public class SockOptVector : global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IEnumerable<SockOpt>
 {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal SockOptVector(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(SockOptVector obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~SockOptVector() {
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
          pjsua2PINVOKE.delete_SockOptVector(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public SockOptVector(global::System.Collections.IEnumerable c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (SockOpt element in c) {
      this.Add(element);
    }
  }

  public SockOptVector(global::System.Collections.Generic.IEnumerable<SockOpt> c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (SockOpt element in c) {
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

  public SockOpt this[int index]  {
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
      if (value < size())
        throw new global::System.ArgumentOutOfRangeException("Capacity");
      reserve((uint)value);
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

  public void CopyTo(SockOpt[] array)
  {
    CopyTo(0, array, 0, this.Count);
  }

  public void CopyTo(SockOpt[] array, int arrayIndex)
  {
    CopyTo(0, array, arrayIndex, this.Count);
  }

  public void CopyTo(int index, SockOpt[] array, int arrayIndex, int count)
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

  public SockOpt[] ToArray() {
    SockOpt[] array = new SockOpt[this.Count];
    this.CopyTo(array);
    return array;
  }

  global::System.Collections.Generic.IEnumerator<SockOpt> global::System.Collections.Generic.IEnumerable<SockOpt>.GetEnumerator() {
    return new SockOptVectorEnumerator(this);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
    return new SockOptVectorEnumerator(this);
  }

  public SockOptVectorEnumerator GetEnumerator() {
    return new SockOptVectorEnumerator(this);
  }

  // Type-safe enumerator
  /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
  /// whenever the collection is modified. This has been done for changes in the size of the
  /// collection but not when one of the elements of the collection is modified as it is a bit
  /// tricky to detect unmanaged code that modifies the collection under our feet.
  public sealed class SockOptVectorEnumerator : global::System.Collections.IEnumerator
    , global::System.Collections.Generic.IEnumerator<SockOpt>
  {
    private SockOptVector collectionRef;
    private int currentIndex;
    private object currentObject;
    private int currentSize;

    public SockOptVectorEnumerator(SockOptVector collection) {
      collectionRef = collection;
      currentIndex = -1;
      currentObject = null;
      currentSize = collectionRef.Count;
    }

    // Type-safe iterator Current
    public SockOpt Current {
      get {
        if (currentIndex == -1)
          throw new global::System.InvalidOperationException("Enumeration not started.");
        if (currentIndex > currentSize - 1)
          throw new global::System.InvalidOperationException("Enumeration finished.");
        if (currentObject == null)
          throw new global::System.InvalidOperationException("Collection modified.");
        return (SockOpt)currentObject;
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

  public void Clear() {
    pjsua2PINVOKE.SockOptVector_Clear(swigCPtr);
  }

  public void Add(SockOpt x) {
    pjsua2PINVOKE.SockOptVector_Add(swigCPtr, SockOpt.getCPtr(x));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  private uint size() {
    uint ret = pjsua2PINVOKE.SockOptVector_size(swigCPtr);
    return ret;
  }

  private uint capacity() {
    uint ret = pjsua2PINVOKE.SockOptVector_capacity(swigCPtr);
    return ret;
  }

  private void reserve(uint n) {
    pjsua2PINVOKE.SockOptVector_reserve(swigCPtr, n);
  }

  public SockOptVector() : this(pjsua2PINVOKE.new_SockOptVector__SWIG_0(), true) {
  }

  public SockOptVector(SockOptVector other) : this(pjsua2PINVOKE.new_SockOptVector__SWIG_1(SockOptVector.getCPtr(other)), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public SockOptVector(int capacity) : this(pjsua2PINVOKE.new_SockOptVector__SWIG_2(capacity), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  private SockOpt getitemcopy(int index) {
    SockOpt ret = new SockOpt(pjsua2PINVOKE.SockOptVector_getitemcopy(swigCPtr, index), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private SockOpt getitem(int index) {
    SockOpt ret = new SockOpt(pjsua2PINVOKE.SockOptVector_getitem(swigCPtr, index), false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void setitem(int index, SockOpt val) {
    pjsua2PINVOKE.SockOptVector_setitem(swigCPtr, index, SockOpt.getCPtr(val));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void AddRange(SockOptVector values) {
    pjsua2PINVOKE.SockOptVector_AddRange(swigCPtr, SockOptVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public SockOptVector GetRange(int index, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.SockOptVector_GetRange(swigCPtr, index, count);
    SockOptVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new SockOptVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Insert(int index, SockOpt x) {
    pjsua2PINVOKE.SockOptVector_Insert(swigCPtr, index, SockOpt.getCPtr(x));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void InsertRange(int index, SockOptVector values) {
    pjsua2PINVOKE.SockOptVector_InsertRange(swigCPtr, index, SockOptVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAt(int index) {
    pjsua2PINVOKE.SockOptVector_RemoveAt(swigCPtr, index);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveRange(int index, int count) {
    pjsua2PINVOKE.SockOptVector_RemoveRange(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public static SockOptVector Repeat(SockOpt value, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.SockOptVector_Repeat(SockOpt.getCPtr(value), count);
    SockOptVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new SockOptVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Reverse() {
    pjsua2PINVOKE.SockOptVector_Reverse__SWIG_0(swigCPtr);
  }

  public void Reverse(int index, int count) {
    pjsua2PINVOKE.SockOptVector_Reverse__SWIG_1(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetRange(int index, SockOptVector values) {
    pjsua2PINVOKE.SockOptVector_SetRange(swigCPtr, index, SockOptVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

}

}
