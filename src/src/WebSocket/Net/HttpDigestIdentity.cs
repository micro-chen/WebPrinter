

using System;
using System.Collections.Specialized;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
  /// <summary>
  /// Holds the user name and other parameters from the HTTP Digest authentication credentials.
  /// </summary>
  public class HttpDigestIdentity : GenericIdentity
  {
    #region Private Fields

    private NameValueCollection _parameters;

    #endregion

    #region Internal Constructors

    internal HttpDigestIdentity (NameValueCollection parameters)
      : base (parameters ["username"], "Digest")
    {
      _parameters = parameters;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the algorithm parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the algorithm parameter.
    /// </value>
    public string Algorithm {
      get {
        return _parameters ["algorithm"];
      }
    }

    /// <summary>
    /// Gets the cnonce parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the cnonce parameter.
    /// </value>
    public string Cnonce {
      get {
        return _parameters ["cnonce"];
      }
    }

    /// <summary>
    /// Gets the nc parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the nc parameter.
    /// </value>
    public string Nc {
      get {
        return _parameters ["nc"];
      }
    }

    /// <summary>
    /// Gets the nonce parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the nonce parameter.
    /// </value>
    public string Nonce {
      get {
        return _parameters ["nonce"];
      }
    }

    /// <summary>
    /// Gets the opaque parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the opaque parameter.
    /// </value>
    public string Opaque {
      get {
        return _parameters ["opaque"];
      }
    }

    /// <summary>
    /// Gets the qop parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the qop parameter.
    /// </value>
    public string Qop {
      get {
        return _parameters ["qop"];
      }
    }

    /// <summary>
    /// Gets the realm parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the realm parameter.
    /// </value>
    public string Realm {
      get {
        return _parameters ["realm"];
      }
    }

    /// <summary>
    /// Gets the response parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the response parameter.
    /// </value>
    public string Response {
      get {
        return _parameters ["response"];
      }
    }

    /// <summary>
    /// Gets the uri parameter from the HTTP Digest authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the uri parameter.
    /// </value>
    public string Uri {
      get {
        return _parameters ["uri"];
      }
    }

    #endregion

    #region Internal Methods

    internal bool IsValid (string password, string realm, string method, string entity)
    {
      var parameters = new NameValueCollection (_parameters);
      parameters ["password"] = password;
      parameters ["realm"] = realm;
      parameters ["method"] = method;
      parameters ["entity"] = entity;

      return _parameters ["response"] == AuthenticationResponse.CreateRequestDigest (parameters);
    }

    #endregion
  }
}
