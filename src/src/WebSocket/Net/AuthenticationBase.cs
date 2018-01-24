

using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
  internal abstract class AuthenticationBase
  {
    #region Private Fields

    private AuthenticationSchemes _scheme;

    #endregion

    #region Internal Fields

    internal NameValueCollection Parameters;

    #endregion

    #region Protected Constructors

    protected AuthenticationBase (AuthenticationSchemes scheme, NameValueCollection parameters)
    {
      _scheme = scheme;
      Parameters = parameters;
    }

    #endregion

    #region Public Properties

    public string Algorithm {
      get {
        return Parameters["algorithm"];
      }
    }

    public string Nonce {
      get {
        return Parameters["nonce"];
      }
    }

    public string Opaque {
      get {
        return Parameters["opaque"];
      }
    }

    public string Qop {
      get {
        return Parameters["qop"];
      }
    }

    public string Realm {
      get {
        return Parameters["realm"];
      }
    }

    public AuthenticationSchemes Scheme {
      get {
        return _scheme;
      }
    }

    #endregion

    #region Internal Methods

    internal static string CreateNonceValue ()
    {
      var src = new byte[16];
      var rand = new Random ();
      rand.NextBytes (src);

      var res = new StringBuilder (32);
      foreach (var b in src)
        res.Append (b.ToString ("x2"));

      return res.ToString ();
    }

    internal static NameValueCollection ParseParameters (string value)
    {
      var res = new NameValueCollection ();
      foreach (var param in value.SplitHeaderValue (',')) {
        var i = param.IndexOf ('=');
        var name = i > 0 ? param.Substring (0, i).Trim () : null;
        var val = i < 0
                  ? param.Trim ().Trim ('"')
                  : i < param.Length - 1
                    ? param.Substring (i + 1).Trim ().Trim ('"')
                    : String.Empty;

        res.Add (name, val);
      }

      return res;
    }

    internal abstract string ToBasicString ();

    internal abstract string ToDigestString ();

    #endregion

    #region Public Methods

    public override string ToString ()
    {
      return _scheme == AuthenticationSchemes.Basic
             ? ToBasicString ()
             : _scheme == AuthenticationSchemes.Digest
               ? ToDigestString ()
               : String.Empty;
    }

    #endregion
  }
}
