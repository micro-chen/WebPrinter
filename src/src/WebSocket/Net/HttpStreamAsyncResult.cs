

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

using System;
using System.Threading;

namespace WebSocketSharp.Net
{
  internal class HttpStreamAsyncResult : IAsyncResult
  {
    #region Private Fields

    private byte[]           _buffer;
    private AsyncCallback    _callback;
    private bool             _completed;
    private int              _count;
    private Exception        _exception;
    private int              _offset;
    private object           _state;
    private object           _sync;
    private int              _syncRead;
    private ManualResetEvent _waitHandle;

    #endregion

    #region Internal Constructors

    internal HttpStreamAsyncResult (AsyncCallback callback, object state)
    {
      _callback = callback;
      _state = state;
      _sync = new object ();
    }

    #endregion

    #region Internal Properties

    internal byte[] Buffer {
      get {
        return _buffer;
      }

      set {
        _buffer = value;
      }
    }

    internal int Count {
      get {
        return _count;
      }

      set {
        _count = value;
      }
    }

    internal Exception Exception {
      get {
        return _exception;
      }
    }

    internal bool HasException {
      get {
        return _exception != null;
      }
    }

    internal int Offset {
      get {
        return _offset;
      }

      set {
        _offset = value;
      }
    }

    internal int SyncRead {
      get {
        return _syncRead;
      }

      set {
        _syncRead = value;
      }
    }

    #endregion

    #region Public Properties

    public object AsyncState {
      get {
        return _state;
      }
    }

    public WaitHandle AsyncWaitHandle {
      get {
        lock (_sync)
          return _waitHandle ?? (_waitHandle = new ManualResetEvent (_completed));
      }
    }

    public bool CompletedSynchronously {
      get {
        return _syncRead == _count;
      }
    }

    public bool IsCompleted {
      get {
        lock (_sync)
          return _completed;
      }
    }

    #endregion

    #region Internal Methods

    internal void Complete ()
    {
      lock (_sync) {
        if (_completed)
          return;

        _completed = true;
        if (_waitHandle != null)
          _waitHandle.Set ();

        if (_callback != null)
          _callback.BeginInvoke (this, ar => _callback.EndInvoke (ar), null);
      }
    }

    internal void Complete (Exception exception)
    {
      _exception = exception;
      Complete ();
    }

    #endregion
  }
}
