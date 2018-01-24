

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace WebSocketSharp.Net
{
  /// <summary>
  /// The exception that is thrown when a <see cref="HttpListener"/> gets an error
  /// processing an HTTP request.
  /// </summary>
  [Serializable]
  public class HttpListenerException : Win32Exception
  {
    #region Protected Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class from
    /// the specified <see cref="SerializationInfo"/> and <see cref="StreamingContext"/>.
    /// </summary>
    /// <param name="serializationInfo">
    /// A <see cref="SerializationInfo"/> that contains the serialized object data.
    /// </param>
    /// <param name="streamingContext">
    /// A <see cref="StreamingContext"/> that specifies the source for the deserialization.
    /// </param>
    protected HttpListenerException (
      SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base (serializationInfo, streamingContext)
    {
    }

    #endregion

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class.
    /// </summary>
    public HttpListenerException ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class
    /// with the specified <paramref name="errorCode"/>.
    /// </summary>
    /// <param name="errorCode">
    /// An <see cref="int"/> that identifies the error.
    /// </param>
    public HttpListenerException (int errorCode)
      : base (errorCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class
    /// with the specified <paramref name="errorCode"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="errorCode">
    /// An <see cref="int"/> that identifies the error.
    /// </param>
    /// <param name="message">
    /// A <see cref="string"/> that describes the error.
    /// </param>
    public HttpListenerException (int errorCode, string message)
      : base (errorCode, message)
    {
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the error code that identifies the error that occurred.
    /// </summary>
    /// <value>
    /// An <see cref="int"/> that identifies the error.
    /// </value>
    public override int ErrorCode {
      get {
        return NativeErrorCode;
      }
    }

    #endregion
  }
}
