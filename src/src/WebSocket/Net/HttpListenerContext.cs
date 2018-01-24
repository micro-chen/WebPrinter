

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

using System;
using System.Security.Principal;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Net
{
  /// <summary>
  /// Provides the access to the HTTP request and response objects used by
  /// the <see cref="HttpListener"/>.
  /// </summary>
  /// <remarks>
  /// This class cannot be inherited.
  /// </remarks>
  public sealed class HttpListenerContext
  {
    #region Private Fields

    private HttpConnection               _connection;
    private string                       _error;
    private int                          _errorStatus;
    private HttpListener                 _listener;
    private HttpListenerRequest          _request;
    private HttpListenerResponse         _response;
    private IPrincipal                   _user;
    private HttpListenerWebSocketContext _websocketContext;

    #endregion

    #region Internal Constructors

    internal HttpListenerContext (HttpConnection connection)
    {
      _connection = connection;
      _errorStatus = 400;
      _request = new HttpListenerRequest (this);
      _response = new HttpListenerResponse (this);
    }

    #endregion

    #region Internal Properties

    internal HttpConnection Connection {
      get {
        return _connection;
      }
    }

    internal string ErrorMessage {
      get {
        return _error;
      }

      set {
        _error = value;
      }
    }

    internal int ErrorStatus {
      get {
        return _errorStatus;
      }

      set {
        _errorStatus = value;
      }
    }

    internal bool HasError {
      get {
        return _error != null;
      }
    }

    internal HttpListener Listener {
      get {
        return _listener;
      }

      set {
        _listener = value;
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the HTTP request object that represents a client request.
    /// </summary>
    /// <value>
    /// A <see cref="HttpListenerRequest"/> that represents the client request.
    /// </value>
    public HttpListenerRequest Request {
      get {
        return _request;
      }
    }

    /// <summary>
    /// Gets the HTTP response object used to send a response to the client.
    /// </summary>
    /// <value>
    /// A <see cref="HttpListenerResponse"/> that represents a response to the client request.
    /// </value>
    public HttpListenerResponse Response {
      get {
        return _response;
      }
    }

    /// <summary>
    /// Gets the client information (identity, authentication, and security roles).
    /// </summary>
    /// <value>
    /// A <see cref="IPrincipal"/> instance that represents the client information.
    /// </value>
    public IPrincipal User {
      get {
        return _user;
      }
    }

    #endregion

    #region Internal Methods

    internal bool Authenticate ()
    {
      var schm = _listener.SelectAuthenticationScheme (_request);
      if (schm == AuthenticationSchemes.Anonymous)
        return true;

      if (schm == AuthenticationSchemes.None) {
        _response.Close (HttpStatusCode.Forbidden);
        return false;
      }

      var realm = _listener.GetRealm ();
      var user =
        HttpUtility.CreateUser (
          _request.Headers["Authorization"],
          schm,
          realm,
          _request.HttpMethod,
          _listener.GetUserCredentialsFinder ()
        );

      if (user == null || !user.Identity.IsAuthenticated) {
        _response.CloseWithAuthChallenge (new AuthenticationChallenge (schm, realm).ToString ());
        return false;
      }

      _user = user;
      return true;
    }

    internal bool Register ()
    {
      return _listener.RegisterContext (this);
    }

    internal void Unregister ()
    {
      _listener.UnregisterContext (this);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Accepts a WebSocket handshake request.
    /// </summary>
    /// <returns>
    /// A <see cref="HttpListenerWebSocketContext"/> that represents
    /// the WebSocket handshake request.
    /// </returns>
    /// <param name="protocol">
    /// A <see cref="string"/> that represents the subprotocol supported on
    /// this WebSocket connection.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="protocol"/> is empty.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="protocol"/> contains an invalid character.
    ///   </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// This method has already been called.
    /// </exception>
    public HttpListenerWebSocketContext AcceptWebSocket (string protocol)
    {
      if (_websocketContext != null)
        throw new InvalidOperationException ("The accepting is already in progress.");

      if (protocol != null) {
        if (protocol.Length == 0)
          throw new ArgumentException ("An empty string.", "protocol");

        if (!protocol.IsToken ())
          throw new ArgumentException ("Contains an invalid character.", "protocol");
      }

      _websocketContext = new HttpListenerWebSocketContext (this, protocol);
      return _websocketContext;
    }

    #endregion
  }
}
