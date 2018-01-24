

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@ximian.com>
 */
#endregion

using System;

namespace WebSocketSharp.Net
{
  internal enum InputChunkState
  {
    None,
    Data,
    DataEnded,
    Trailer,
    End
  }
}
