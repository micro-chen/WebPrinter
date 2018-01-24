

#region Authors
/*
 * Authors:
 * - Patrik Torstensson <Patrik.Torstensson@labs2.com>
 * - Wictor Wilén (decode/encode functions) <wictor@ibizkit.se>
 * - Tim Coleman <tim@timcoleman.com>
 * - Gonzalo Paniagua Javier <gonzalo@ximian.com>
 */
#endregion

using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
  internal sealed class QueryStringCollection : NameValueCollection
  {
    public override string ToString ()
    {
      var cnt = Count;
      if (cnt == 0)
        return String.Empty;

      var output = new StringBuilder ();
      var keys = AllKeys;
      foreach (var key in keys)
        output.AppendFormat ("{0}={1}&", key, this [key]);

      if (output.Length > 0)
        output.Length--;

      return output.ToString ();
    }
  }
}
