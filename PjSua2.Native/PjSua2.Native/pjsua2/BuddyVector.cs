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

public class BuddyVector : global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IList<Buddy>
 {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal BuddyVector(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(BuddyVector obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~BuddyVector() {
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
          pjsua2PINVOKE.delete_BuddyVector(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public BuddyVector(global::System.Collections.IEnumerable c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (Buddy element in c) {
      this.Add(element);
    }
  }

  public BuddyVector(global::System.Collections.Generic.IEnumerable<Buddy> c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (Buddy element in c) {
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

  public Buddy this[int index]  {
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

  public void CopyTo(Buddy[] array)
  {
    CopyTo(0, array, 0, this.Count);
  }

  public void CopyTo(Buddy[] array, int arrayIndex)
  {
    CopyTo(0, array, arrayIndex, this.Count);
  }

  public void CopyTo(int index, Buddy[] array, int arrayIndex, int count)
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

  public Buddy[] ToArray() {
    Buddy[] array = new Buddy[this.Count];
    this.CopyTo(array);
    return array;
  }

  global::System.Collections.Generic.IEnumerator<Buddy> global::System.Collections.Generic.IEnumerable<Buddy>.GetEnumerator() {
    return new BuddyVectorEnumerator(this);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
    return new BuddyVectorEnumerator(this);
  }

  public BuddyVectorEnumerator GetEnumerator() {
    return new BuddyVectorEnumerator(this);
  }

  // Type-safe enumerator
  /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
  /// whenever the collection is modified. This has been done for changes in the size of the
  /// collection but not when one of the elements of the collection is modified as it is a bit
  /// tricky to detect unmanaged code that modifies the collection under our feet.
  public sealed class BuddyVectorEnumerator : global::System.Collections.IEnumerator
    , global::System.Collections.Generic.IEnumerator<Buddy>
  {
    private BuddyVector collectionRef;
    private int currentIndex;
    private object currentObject;
    private int currentSize;

    public BuddyVectorEnumerator(BuddyVector collection) {
      collectionRef = collection;
      currentIndex = -1;
      currentObject = null;
      currentSize = collectionRef.Count;
    }

    // Type-safe iterator Current
    public Buddy Current {
      get {
        if (currentIndex == -1)
          throw new global::System.InvalidOperationException("Enumeration not started.");
        if (currentIndex > currentSize - 1)
          throw new global::System.InvalidOperationException("Enumeration finished.");
        if (currentObject == null)
          throw new global::System.InvalidOperationException("Collection modified.");
        return (Buddy)currentObject;
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
    pjsua2PINVOKE.BuddyVector_Clear(swigCPtr);
  }

  public void Add(Buddy x) {
    pjsua2PINVOKE.BuddyVector_Add(swigCPtr, Buddy.getCPtr(x));
  }

  private uint size() {
    uint ret = pjsua2PINVOKE.BuddyVector_size(swigCPtr);
    return ret;
  }

  private uint capacity() {
    uint ret = pjsua2PINVOKE.BuddyVector_capacity(swigCPtr);
    return ret;
  }

  private void reserve(uint n) {
    pjsua2PINVOKE.BuddyVector_reserve(swigCPtr, n);
  }

  public BuddyVector() : this(pjsua2PINVOKE.new_BuddyVector__SWIG_0(), true) {
  }

  public BuddyVector(BuddyVector other) : this(pjsua2PINVOKE.new_BuddyVector__SWIG_1(BuddyVector.getCPtr(other)), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public BuddyVector(int capacity) : this(pjsua2PINVOKE.new_BuddyVector__SWIG_2(capacity), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  private Buddy getitemcopy(int index) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.BuddyVector_getitemcopy(swigCPtr, index);
    Buddy ret = (cPtr == global::System.IntPtr.Zero) ? null : new Buddy(cPtr, false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private Buddy getitem(int index) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.BuddyVector_getitem(swigCPtr, index);
    Buddy ret = (cPtr == global::System.IntPtr.Zero) ? null : new Buddy(cPtr, false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void setitem(int index, Buddy val) {
    pjsua2PINVOKE.BuddyVector_setitem(swigCPtr, index, Buddy.getCPtr(val));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void AddRange(BuddyVector values) {
    pjsua2PINVOKE.BuddyVector_AddRange(swigCPtr, BuddyVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public BuddyVector GetRange(int index, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.BuddyVector_GetRange(swigCPtr, index, count);
    BuddyVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new BuddyVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Insert(int index, Buddy x) {
    pjsua2PINVOKE.BuddyVector_Insert(swigCPtr, index, Buddy.getCPtr(x));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void InsertRange(int index, BuddyVector values) {
    pjsua2PINVOKE.BuddyVector_InsertRange(swigCPtr, index, BuddyVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAt(int index) {
    pjsua2PINVOKE.BuddyVector_RemoveAt(swigCPtr, index);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveRange(int index, int count) {
    pjsua2PINVOKE.BuddyVector_RemoveRange(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public static BuddyVector Repeat(Buddy value, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.BuddyVector_Repeat(Buddy.getCPtr(value), count);
    BuddyVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new BuddyVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Reverse() {
    pjsua2PINVOKE.BuddyVector_Reverse__SWIG_0(swigCPtr);
  }

  public void Reverse(int index, int count) {
    pjsua2PINVOKE.BuddyVector_Reverse__SWIG_1(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetRange(int index, BuddyVector values) {
    pjsua2PINVOKE.BuddyVector_SetRange(swigCPtr, index, BuddyVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public bool Contains(Buddy value) {
    bool ret = pjsua2PINVOKE.BuddyVector_Contains(swigCPtr, Buddy.getCPtr(value));
    return ret;
  }

  public int IndexOf(Buddy value) {
    int ret = pjsua2PINVOKE.BuddyVector_IndexOf(swigCPtr, Buddy.getCPtr(value));
    return ret;
  }

  public int LastIndexOf(Buddy value) {
    int ret = pjsua2PINVOKE.BuddyVector_LastIndexOf(swigCPtr, Buddy.getCPtr(value));
    return ret;
  }

  public bool Remove(Buddy value) {
    bool ret = pjsua2PINVOKE.BuddyVector_Remove(swigCPtr, Buddy.getCPtr(value));
    return ret;
  }

}

}
