

using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
  internal class AuthenticationChallenge : AuthenticationBase
  {
    #region Private Constructors

    private AuthenticationChallenge (AuthenticationSchemes scheme, NameValueCollection parameters)
      : base (scheme, parameters)
    {
    }

    #endregion

    #region Internal Constructors

    internal AuthenticationChallenge (AuthenticationSchemes scheme, string realm)
      : base (scheme, new NameValueCollection ())
    {
      Parameters["realm"] = realm;
      if (scheme == AuthenticationSchemes.Digest) {
        Parameters["nonce"] = CreateNonceValue ();
        Parameters["algorithm"] = "MD5";
        Parameters["qop"] = "auth";
      }
    }

    #endregion

    #region Public Properties

    public string Domain {
      get {
        return Parameters["domain"];
      }
    }

    public string Stale {
      get {
        return Parameters["stale"];
      }
    }

    #endregion

    #region Internal Methods

    internal static AuthenticationChallenge CreateBasicChallenge (string realm)
    {
      return new AuthenticationChallenge (AuthenticationSchemes.Basic, realm);
    }

    internal static AuthenticationChallenge CreateDigestChallenge (string realm)
    {
      return new AuthenticationChallenge (AuthenticationSchemes.Digest, realm);
    }

    internal static AuthenticationChallenge Parse (string value)
    {
      var chal = value.Split (new[] { ' ' }, 2);
      if (chal.Length != 2)
        return null;

      var schm = chal[0].ToLower ();
      return schm == "basic"
             ? new AuthenticationChallenge (
                 AuthenticationSchemes.Basic, ParseParameters (chal[1]))
             : schm == "digest"
               ? new AuthenticationChallenge (
                   AuthenticationSchemes.Digest, ParseParameters (chal[1]))
               : null;
    }

    internal override string ToBasicString ()
    {
      return String.Format ("Basic realm=\"{0}\"", Parameters["realm"]);
    }

    internal override string ToDigestString ()
    {
      var output = new StringBuilder (128);

      var domain = Parameters["domain"];
      if (domain != null)
        output.AppendFormat (
          "Digest realm=\"{0}\", domain=\"{1}\", nonce=\"{2}\"",
          Parameters["realm"],
          domain,
          Parameters["nonce"]);
      else
        output.AppendFormat (
          "Digest realm=\"{0}\", nonce=\"{1}\"", Parameters["realm"], Parameters["nonce"]);

      var opaque = Parameters["opaque"];
      if (opaque != null)
        output.AppendFormat (", opaque=\"{0}\"", opaque);

      var stale = Parameters["stale"];
      if (stale != null)
        output.AppendFormat (", stale={0}", stale);

      var algo = Parameters["algorithm"];
      if (algo != null)
        output.AppendFormat (", algorithm={0}", algo);

      var qop = Parameters["qop"];
      if (qop != null)
        output.AppendFormat (", qop=\"{0}\"", qop);

      return output.ToString ();
    }

    #endregion
  }
}
