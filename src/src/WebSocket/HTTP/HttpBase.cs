

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
  internal abstract class HttpBase
  {
    #region Private Fields

    private NameValueCollection _headers;
    private const int           _headersMaxLength = 8192;
    private Version             _version;

    #endregion

    #region Internal Fields

    internal byte[] EntityBodyData;

    #endregion

    #region Protected Fields

    protected const string CrLf = "\r\n";

    #endregion

    #region Protected Constructors

    protected HttpBase (Version version, NameValueCollection headers)
    {
      _version = version;
      _headers = headers;
    }

    #endregion

    #region Public Properties

    public string EntityBody {
      get {
        if (EntityBodyData == null || EntityBodyData.LongLength == 0)
          return String.Empty;

        Encoding enc = null;

        var contentType = _headers["Content-Type"];
        if (contentType != null && contentType.Length > 0)
          enc = HttpUtility.GetEncoding (contentType);

        return (enc ?? Encoding.UTF8).GetString (EntityBodyData);
      }
    }

    public NameValueCollection Headers {
      get {
        return _headers;
      }
    }

    public Version ProtocolVersion {
      get {
        return _version;
      }
    }

    #endregion

    #region Private Methods

    private static byte[] readEntityBody (Stream stream, string length)
    {
      long len;
      if (!Int64.TryParse (length, out len))
        throw new ArgumentException ("Cannot be parsed.", "length");

      if (len < 0)
        throw new ArgumentOutOfRangeException ("length", "Less than zero.");

      return len > 1024
             ? stream.ReadBytes (len, 1024)
             : len > 0
               ? stream.ReadBytes ((int) len)
               : null;
    }

    private static string[] readHeaders (Stream stream, int maxLength)
    {
      var buff = new List<byte> ();
      var cnt = 0;
      Action<int> add = i => {
        if (i == -1)
          throw new EndOfStreamException ("The header cannot be read from the data source.");

        buff.Add ((byte) i);
        cnt++;
      };

      var read = false;
      while (cnt < maxLength) {
        if (stream.ReadByte ().EqualsWith ('\r', add) &&
            stream.ReadByte ().EqualsWith ('\n', add) &&
            stream.ReadByte ().EqualsWith ('\r', add) &&
            stream.ReadByte ().EqualsWith ('\n', add)) {
          read = true;
          break;
        }
      }

      if (!read)
        throw new WebSocketException ("The length of header part is greater than the max length.");

      return Encoding.UTF8.GetString (buff.ToArray ())
             .Replace (CrLf + " ", " ")
             .Replace (CrLf + "\t", " ")
             .Split (new[] { CrLf }, StringSplitOptions.RemoveEmptyEntries);
    }

    #endregion

    #region Protected Methods

    protected static T Read<T> (Stream stream, Func<string[], T> parser, int millisecondsTimeout)
      where T : HttpBase
    {
      var timeout = false;
      var timer = new Timer (
        state => {
          timeout = true;
          stream.Close ();
        },
        null,
        millisecondsTimeout,
        -1);

      T http = null;
      Exception exception = null;
      try {
        http = parser (readHeaders (stream, _headersMaxLength));
        var contentLen = http.Headers["Content-Length"];
        if (contentLen != null && contentLen.Length > 0)
          http.EntityBodyData = readEntityBody (stream, contentLen);
      }
      catch (Exception ex) {
        exception = ex;
      }
      finally {
        timer.Change (-1, -1);
        timer.Dispose ();
      }

      var msg = timeout
                ? "A timeout has occurred while reading an HTTP request/response."
                : exception != null
                  ? "An exception has occurred while reading an HTTP request/response."
                  : null;

      if (msg != null)
        throw new WebSocketException (msg, exception);

      return http;
    }

    #endregion

    #region Public Methods

    public byte[] ToByteArray ()
    {
      return Encoding.UTF8.GetBytes (ToString ());
    }
    
    #endregion
  }
}
