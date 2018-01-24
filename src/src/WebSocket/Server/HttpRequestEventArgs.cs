

using System;
using WebSocketSharp.Net;

namespace WebSocketSharp.Server
{
  /// <summary>
  /// Represents the event data for the HTTP request event that the <see cref="HttpServer"/> emits.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   An HTTP request event occurs when the <see cref="HttpServer"/> receives an HTTP request.
  ///   </para>
  ///   <para>
  ///   If you would like to get the request data sent from a client,
  ///   you should access the <see cref="Request"/> property.
  ///   </para>
  ///   <para>
  ///   And if you would like to get the response data used to return a response,
  ///   you should access the <see cref="Response"/> property.
  ///   </para>
  /// </remarks>
  public class HttpRequestEventArgs : EventArgs
  {
    #region Private Fields

    private HttpListenerRequest  _request;
    private HttpListenerResponse _response;

    #endregion

    #region Internal Constructors

    internal HttpRequestEventArgs (HttpListenerContext context)
    {
      _request = context.Request;
      _response = context.Response;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the HTTP request data sent from a client.
    /// </summary>
    /// <value>
    /// A <see cref="HttpListenerRequest"/> that represents the request data.
    /// </value>
    public HttpListenerRequest Request {
      get {
        return _request;
      }
    }

    /// <summary>
    /// Gets the HTTP response data used to return a response to the client.
    /// </summary>
    /// <value>
    /// A <see cref="HttpListenerResponse"/> that represents the response data.
    /// </value>
    public HttpListenerResponse Response {
      get {
        return _response;
      }
    }

    #endregion
  }
}
