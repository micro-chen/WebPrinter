

using System;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
  /// <summary>
  /// Exposes the properties used to access the information in a session in a WebSocket service.
  /// </summary>
  public interface IWebSocketSession
  {
    #region Properties

    /// <summary>
    /// Gets the information in the connection request to the WebSocket service.
    /// </summary>
    /// <value>
    /// A <see cref="WebSocketContext"/> that provides the access to the connection request.
    /// </value>
    WebSocketContext Context { get; }

    /// <summary>
    /// Gets the unique ID of the session.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the unique ID of the session.
    /// </value>
    string ID { get; }

    /// <summary>
    /// Gets the WebSocket subprotocol used in the session.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the subprotocol if any.
    /// </value>
    string Protocol { get; }

    /// <summary>
    /// Gets the time that the session has started.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> that represents the time that the session has started.
    /// </value>
    DateTime StartTime { get; }

    /// <summary>
    /// Gets the state of the <see cref="WebSocket"/> used in the session.
    /// </summary>
    /// <value>
    /// One of the <see cref="WebSocketState"/> enum values, indicates the state of
    /// the <see cref="WebSocket"/> used in the session.
    /// </value>
    WebSocketState State { get; }

    #endregion
  }
}
