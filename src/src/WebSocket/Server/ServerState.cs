

using System;

namespace WebSocketSharp.Server
{
  internal enum ServerState
  {
    Ready,
    Start,
    ShuttingDown,
    Stop
  }
}
