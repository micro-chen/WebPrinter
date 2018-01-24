

using System;

namespace WebSocketSharp
{
  /// <summary>
  /// Specifies the compression method used to compress a message on the WebSocket connection.
  /// </summary>
  /// <remarks>
  /// The compression methods are defined in
  /// <see href="http://tools.ietf.org/html/draft-ietf-hybi-permessage-compression-19">
  /// Compression Extensions for WebSocket</see>.
  /// </remarks>
  public enum CompressionMethod : byte
  {
    /// <summary>
    /// Specifies non compression.
    /// </summary>
    None,
    /// <summary>
    /// Specifies DEFLATE.
    /// </summary>
    Deflate
  }
}
