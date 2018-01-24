

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

using System;

namespace WebSocketSharp.Net
{
  internal enum LineState
  {
    None,
    Cr,
    Lf
  }
}
