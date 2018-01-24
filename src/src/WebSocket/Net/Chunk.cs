

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@ximian.com>
 */
#endregion

using System;

namespace WebSocketSharp.Net
{
  internal class Chunk
  {
    #region Private Fields

    private byte[] _data;
    private int    _offset;

    #endregion

    #region Public Constructors

    public Chunk (byte[] data)
    {
      _data = data;
    }

    #endregion

    #region Public Properties

    public int ReadLeft {
      get {
        return _data.Length - _offset;
      }
    }

    #endregion

    #region Public Methods

    public int Read (byte[] buffer, int offset, int count)
    {
      var left = _data.Length - _offset;
      if (left == 0)
        return left;

      if (count > left)
        count = left;

      Buffer.BlockCopy (_data, _offset, buffer, offset, count);
      _offset += count;

      return count;
    }

    #endregion
  }
}
