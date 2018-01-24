

#region Authors
/*
 * Authors:
 * - Atsushi Enomoto <atsushi@ximian.com>
 */
#endregion

using System;

namespace WebSocketSharp.Net
{
  /// <summary>
  /// Specifies the scheme for authentication.
  /// </summary>
  public enum AuthenticationSchemes
  {
    /// <summary>
    /// No authentication is allowed.
    /// </summary>
    None,
    /// <summary>
    /// Specifies digest authentication.
    /// </summary>
    Digest = 1,
    /// <summary>
    /// Specifies basic authentication.
    /// </summary>
    Basic = 8,
    /// <summary>
    /// Specifies anonymous authentication.
    /// </summary>
    Anonymous = 0x8000
  }
}
