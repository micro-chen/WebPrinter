

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

namespace WebSocketSharp.Net
{
  /// <summary>
  /// Contains the HTTP headers that can be specified in a server response.
  /// </summary>
  /// <remarks>
  /// The HttpResponseHeader enumeration contains the HTTP response headers defined in
  /// <see href="http://tools.ietf.org/html/rfc2616#section-14">RFC 2616</see> for the HTTP/1.1 and
  /// <see href="http://tools.ietf.org/html/rfc6455#section-11.3">RFC 6455</see> for the WebSocket.
  /// </remarks>
  public enum HttpResponseHeader
  {
    /// <summary>
    /// Indicates the Cache-Control header.
    /// </summary>
    CacheControl,
    /// <summary>
    /// Indicates the Connection header.
    /// </summary>
    Connection,
    /// <summary>
    /// Indicates the Date header.
    /// </summary>
    Date,
    /// <summary>
    /// Indicates the Keep-Alive header.
    /// </summary>
    KeepAlive,
    /// <summary>
    /// Indicates the Pragma header.
    /// </summary>
    Pragma,
    /// <summary>
    /// Indicates the Trailer header.
    /// </summary>
    Trailer,
    /// <summary>
    /// Indicates the Transfer-Encoding header.
    /// </summary>
    TransferEncoding,
    /// <summary>
    /// Indicates the Upgrade header.
    /// </summary>
    Upgrade,
    /// <summary>
    /// Indicates the Via header.
    /// </summary>
    Via,
    /// <summary>
    /// Indicates the Warning header.
    /// </summary>
    Warning,
    /// <summary>
    /// Indicates the Allow header.
    /// </summary>
    Allow,
    /// <summary>
    /// Indicates the Content-Length header.
    /// </summary>
    ContentLength,
    /// <summary>
    /// Indicates the Content-Type header.
    /// </summary>
    ContentType,
    /// <summary>
    /// Indicates the Content-Encoding header.
    /// </summary>
    ContentEncoding,
    /// <summary>
    /// Indicates the Content-Language header.
    /// </summary>
    ContentLanguage,
    /// <summary>
    /// Indicates the Content-Location header.
    /// </summary>
    ContentLocation,
    /// <summary>
    /// Indicates the Content-MD5 header.
    /// </summary>
    ContentMd5,
    /// <summary>
    /// Indicates the Content-Range header.
    /// </summary>
    ContentRange,
    /// <summary>
    /// Indicates the Expires header.
    /// </summary>
    Expires,
    /// <summary>
    /// Indicates the Last-Modified header.
    /// </summary>
    LastModified,
    /// <summary>
    /// Indicates the Accept-Ranges header.
    /// </summary>
    AcceptRanges,
    /// <summary>
    /// Indicates the Age header.
    /// </summary>
    Age,
    /// <summary>
    /// Indicates the ETag header.
    /// </summary>
    ETag,
    /// <summary>
    /// Indicates the Location header.
    /// </summary>
    Location,
    /// <summary>
    /// Indicates the Proxy-Authenticate header.
    /// </summary>
    ProxyAuthenticate,
    /// <summary>
    /// Indicates the Retry-After header.
    /// </summary>
    RetryAfter,
    /// <summary>
    /// Indicates the Server header.
    /// </summary>
    Server,
    /// <summary>
    /// Indicates the Set-Cookie header.
    /// </summary>
    SetCookie,
    /// <summary>
    /// Indicates the Vary header.
    /// </summary>
    Vary,
    /// <summary>
    /// Indicates the WWW-Authenticate header.
    /// </summary>
    WwwAuthenticate,
    /// <summary>
    /// Indicates the Sec-WebSocket-Extensions header.
    /// </summary>
    SecWebSocketExtensions,
    /// <summary>
    /// Indicates the Sec-WebSocket-Accept header.
    /// </summary>
    SecWebSocketAccept,
    /// <summary>
    /// Indicates the Sec-WebSocket-Protocol header.
    /// </summary>
    SecWebSocketProtocol,
    /// <summary>
    /// Indicates the Sec-WebSocket-Version header.
    /// </summary>
    SecWebSocketVersion
  }
}
