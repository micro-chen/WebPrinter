

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@ximian.com>
 */
#endregion

#region Contributors
/*
 * Contributors:
 * - Nicholas Devenish
 */
#endregion

using System;
using System.Threading;

namespace WebSocketSharp.Net
{
  internal class HttpListenerAsyncResult : IAsyncResult
  {
    #region Private Fields

    private AsyncCallback       _callback;
    private bool                _completed;
    private HttpListenerContext _context;
    private bool                _endCalled;
    private Exception           _exception;
    private bool                _inGet;
    private object              _state;
    private object              _sync;
    private bool                _syncCompleted;
    private ManualResetEvent    _waitHandle;

    #endregion

    #region Internal Constructors

    internal HttpListenerAsyncResult (AsyncCallback callback, object state)
    {
      _callback = callback;
      _state = state;
      _sync = new object ();
    }

    #endregion

    #region Internal Properties

    internal bool EndCalled {
      get {
        return _endCalled;
      }

      set {
        _endCalled = value;
      }
    }

    internal bool InGet {
      get {
        return _inGet;
      }

      set {
        _inGet = value;
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
        return _syncCompleted;
      }
    }

    public bool IsCompleted {
      get {
        lock (_sync)
          return _completed;
      }
    }

    #endregion

    #region Private Methods

    private static void complete (HttpListenerAsyncResult asyncResult)
    {
      lock (asyncResult._sync) {
        asyncResult._completed = true;

        var waitHandle = asyncResult._waitHandle;
        if (waitHandle != null)
          waitHandle.Set ();
      }

      var callback = asyncResult._callback;
      if (callback == null)
        return;

      ThreadPool.QueueUserWorkItem (
        state => {
          try {
            callback (asyncResult);
          }
          catch {
          }
        },
        null
      );
    }

    #endregion

    #region Internal Methods

    internal void Complete (Exception exception)
    {
      _exception = _inGet && (exception is ObjectDisposedException)
                   ? new HttpListenerException (995, "The listener is closed.")
                   : exception;

      complete (this);
    }

    internal void Complete (HttpListenerContext context)
    {
      Complete (context, false);
    }

    internal void Complete (HttpListenerContext context, bool syncCompleted)
    {
      _context = context;
      _syncCompleted = syncCompleted;

      complete (this);
    }

    internal HttpListenerContext GetContext ()
    {
      if (_exception != null)
        throw _exception;

      return _context;
    }

    #endregion
  }
}
