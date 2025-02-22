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

public class AudDevManager : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal AudDevManager(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AudDevManager obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  internal static global::System.Runtime.InteropServices.HandleRef swigRelease(AudDevManager obj) {
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

  ~AudDevManager() {
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
          throw new global::System.MethodAccessException("C++ destructor does not have public access");
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int getCaptureDev() {
    int ret = pjsua2PINVOKE.AudDevManager_getCaptureDev(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public AudioMedia getCaptureDevMedia() {
    AudioMedia ret = new AudioMedia(pjsua2PINVOKE.AudDevManager_getCaptureDevMedia(swigCPtr), false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int getPlaybackDev() {
    int ret = pjsua2PINVOKE.AudDevManager_getPlaybackDev(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public AudioMedia getPlaybackDevMedia() {
    AudioMedia ret = new AudioMedia(pjsua2PINVOKE.AudDevManager_getPlaybackDevMedia(swigCPtr), false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setCaptureDev(int capture_dev) {
    pjsua2PINVOKE.AudDevManager_setCaptureDev(swigCPtr, capture_dev);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setPlaybackDev(int playback_dev) {
    pjsua2PINVOKE.AudDevManager_setPlaybackDev(swigCPtr, playback_dev);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public AudioDevInfoVector2 enumDev2() {
    AudioDevInfoVector2 ret = new AudioDevInfoVector2(pjsua2PINVOKE.AudDevManager_enumDev2(swigCPtr), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setNullDev() {
    pjsua2PINVOKE.AudDevManager_setNullDev(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public SWIGTYPE_p_p_void setNoDev() {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AudDevManager_setNoDev(swigCPtr);
    SWIGTYPE_p_p_void ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_p_void(cPtr, false);
    return ret;
  }

  public void setSndDevMode(uint mode) {
    pjsua2PINVOKE.AudDevManager_setSndDevMode(swigCPtr, mode);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setEcOptions(uint tail_msec, uint options) {
    pjsua2PINVOKE.AudDevManager_setEcOptions(swigCPtr, tail_msec, options);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getEcTail() {
    uint ret = pjsua2PINVOKE.AudDevManager_getEcTail(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public bool sndIsActive() {
    bool ret = pjsua2PINVOKE.AudDevManager_sndIsActive(swigCPtr);
    return ret;
  }

  public void refreshDevs() {
    pjsua2PINVOKE.AudDevManager_refreshDevs(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getDevCount() {
    uint ret = pjsua2PINVOKE.AudDevManager_getDevCount(swigCPtr);
    return ret;
  }

  public AudioDevInfo getDevInfo(int id) {
    AudioDevInfo ret = new AudioDevInfo(pjsua2PINVOKE.AudDevManager_getDevInfo(swigCPtr, id), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public int lookupDev(string drv_name, string dev_name) {
    int ret = pjsua2PINVOKE.AudDevManager_lookupDev(swigCPtr, drv_name, dev_name);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public string capName(pjmedia_aud_dev_cap cap) {
    string ret = pjsua2PINVOKE.AudDevManager_capName(swigCPtr, (int)cap);
    return ret;
  }

  public void setExtFormat(MediaFormatAudio format, bool keep) {
    pjsua2PINVOKE.AudDevManager_setExtFormat__SWIG_0(swigCPtr, MediaFormatAudio.getCPtr(format), keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setExtFormat(MediaFormatAudio format) {
    pjsua2PINVOKE.AudDevManager_setExtFormat__SWIG_1(swigCPtr, MediaFormatAudio.getCPtr(format));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public MediaFormatAudio getExtFormat() {
    MediaFormatAudio ret = new MediaFormatAudio(pjsua2PINVOKE.AudDevManager_getExtFormat(swigCPtr), true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setInputLatency(uint latency_msec, bool keep) {
    pjsua2PINVOKE.AudDevManager_setInputLatency__SWIG_0(swigCPtr, latency_msec, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setInputLatency(uint latency_msec) {
    pjsua2PINVOKE.AudDevManager_setInputLatency__SWIG_1(swigCPtr, latency_msec);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getInputLatency() {
    uint ret = pjsua2PINVOKE.AudDevManager_getInputLatency(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setOutputLatency(uint latency_msec, bool keep) {
    pjsua2PINVOKE.AudDevManager_setOutputLatency__SWIG_0(swigCPtr, latency_msec, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setOutputLatency(uint latency_msec) {
    pjsua2PINVOKE.AudDevManager_setOutputLatency__SWIG_1(swigCPtr, latency_msec);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getOutputLatency() {
    uint ret = pjsua2PINVOKE.AudDevManager_getOutputLatency(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setInputVolume(uint volume, bool keep) {
    pjsua2PINVOKE.AudDevManager_setInputVolume__SWIG_0(swigCPtr, volume, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setInputVolume(uint volume) {
    pjsua2PINVOKE.AudDevManager_setInputVolume__SWIG_1(swigCPtr, volume);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getInputVolume() {
    uint ret = pjsua2PINVOKE.AudDevManager_getInputVolume(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setOutputVolume(uint volume, bool keep) {
    pjsua2PINVOKE.AudDevManager_setOutputVolume__SWIG_0(swigCPtr, volume, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setOutputVolume(uint volume) {
    pjsua2PINVOKE.AudDevManager_setOutputVolume__SWIG_1(swigCPtr, volume);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public uint getOutputVolume() {
    uint ret = pjsua2PINVOKE.AudDevManager_getOutputVolume(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public uint getInputSignal() {
    uint ret = pjsua2PINVOKE.AudDevManager_getInputSignal(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public uint getOutputSignal() {
    uint ret = pjsua2PINVOKE.AudDevManager_getOutputSignal(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setInputRoute(pjmedia_aud_dev_route route, bool keep) {
    pjsua2PINVOKE.AudDevManager_setInputRoute__SWIG_0(swigCPtr, (int)route, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setInputRoute(pjmedia_aud_dev_route route) {
    pjsua2PINVOKE.AudDevManager_setInputRoute__SWIG_1(swigCPtr, (int)route);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public pjmedia_aud_dev_route getInputRoute() {
    pjmedia_aud_dev_route ret = (pjmedia_aud_dev_route)pjsua2PINVOKE.AudDevManager_getInputRoute(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setOutputRoute(pjmedia_aud_dev_route route, bool keep) {
    pjsua2PINVOKE.AudDevManager_setOutputRoute__SWIG_0(swigCPtr, (int)route, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setOutputRoute(pjmedia_aud_dev_route route) {
    pjsua2PINVOKE.AudDevManager_setOutputRoute__SWIG_1(swigCPtr, (int)route);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public pjmedia_aud_dev_route getOutputRoute() {
    pjmedia_aud_dev_route ret = (pjmedia_aud_dev_route)pjsua2PINVOKE.AudDevManager_getOutputRoute(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setVad(bool enable, bool keep) {
    pjsua2PINVOKE.AudDevManager_setVad__SWIG_0(swigCPtr, enable, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setVad(bool enable) {
    pjsua2PINVOKE.AudDevManager_setVad__SWIG_1(swigCPtr, enable);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public bool getVad() {
    bool ret = pjsua2PINVOKE.AudDevManager_getVad(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setCng(bool enable, bool keep) {
    pjsua2PINVOKE.AudDevManager_setCng__SWIG_0(swigCPtr, enable, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setCng(bool enable) {
    pjsua2PINVOKE.AudDevManager_setCng__SWIG_1(swigCPtr, enable);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public bool getCng() {
    bool ret = pjsua2PINVOKE.AudDevManager_getCng(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void setPlc(bool enable, bool keep) {
    pjsua2PINVOKE.AudDevManager_setPlc__SWIG_0(swigCPtr, enable, keep);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void setPlc(bool enable) {
    pjsua2PINVOKE.AudDevManager_setPlc__SWIG_1(swigCPtr, enable);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public bool getPlc() {
    bool ret = pjsua2PINVOKE.AudDevManager_getPlc(swigCPtr);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

}

}
