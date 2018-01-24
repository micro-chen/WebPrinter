

#region Contributors
/*
 * Contributors:
 * - Juan Manuel Lallana <juan.manuel.lallana@gmail.com>
 */
#endregion

using System;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
  /// <summary>
  /// Exposes the methods and properties used to access the information in a WebSocket service
  /// provided by the <see cref="WebSocketServer"/> or <see cref="HttpServer"/>.
  /// </summary>
  /// <remarks>
  /// The WebSocketServiceHost class is an abstract class.
  /// </remarks>
  public abstract class WebSocketServiceHost
  {
    #region Protected Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketServiceHost"/> class.
    /// </summary>
    protected WebSocketServiceHost ()
    {
    }

    #endregion

    #region Internal Properties

    internal ServerState State {
      get {
        return Sessions.State;
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets a value indicating whether the WebSocket service cleans up
    /// the inactive sessions periodically.
    /// </summary>
    /// <value>
    /// <c>true</c> if the service cleans up the inactive sessions periodically;
    /// otherwise, <c>false</c>.
    /// </value>
    public abstract bool KeepClean { get; set; }

    /// <summary>
    /// Gets the path to the WebSocket service.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the absolute path to the service.
    /// </value>
    public abstract string Path { get; }

    /// <summary>
    /// Gets the access to the sessions in the WebSocket service.
    /// </summary>
    /// <value>
    /// A <see cref="WebSocketSessionManager"/> that manages the sessions in the service.
    /// </value>
    public abstract WebSocketSessionManager Sessions { get; }

    /// <summary>
    /// Gets the <see cref="System.Type"/> of the behavior of the WebSocket service.
    /// </summary>
    /// <value>
    /// A <see cref="System.Type"/> that represents the type of the behavior of the service.
    /// </value>
    public abstract Type Type { get; }

    /// <summary>
    /// Gets or sets the wait time for the response to the WebSocket Ping or Close.
    /// </summary>
    /// <value>
    /// A <see cref="TimeSpan"/> that represents the wait time. The default value is
    /// the same as 1 second.
    /// </value>
    public abstract TimeSpan WaitTime { get; set; }

    #endregion

    #region Internal Methods

    internal void Start ()
    {
      Sessions.Start ();
    }

    internal void StartSession (WebSocketContext context)
    {
      CreateSession ().Start (context, Sessions);
    }

    internal void Stop (ushort code, string reason)
    {
      var e = new CloseEventArgs (code, reason);
      var send = !code.IsReserved ();
      var bytes = send ? WebSocketFrame.CreateCloseFrame (e.PayloadData, false).ToArray () : null;
      Sessions.Stop (e, bytes, send);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Creates a new session in the WebSocket service.
    /// </summary>
    /// <returns>
    /// A <see cref="WebSocketBehavior"/> instance that represents a new session.
    /// </returns>
    protected abstract WebSocketBehavior CreateSession ();

    #endregion
  }
}
