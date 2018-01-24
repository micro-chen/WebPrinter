

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

using System;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
  /// <summary>
  /// Holds the user name and password from the HTTP Basic authentication credentials.
  /// </summary>
  public class HttpBasicIdentity : GenericIdentity
  {
    #region Private Fields

    private string _password;

    #endregion

    #region internal Constructors

    internal HttpBasicIdentity (string username, string password)
      : base (username, "Basic")
    {
      _password = password;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the password from the HTTP Basic authentication credentials.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that represents the password.
    /// </value>
    public virtual string Password {
      get {
        return _password;
      }
    }

    #endregion
  }
}
