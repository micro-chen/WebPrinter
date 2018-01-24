

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@ximian.com>
 */
#endregion

#region Contributors
/*
 * Contributors:
 * - Liryna <liryna.stark@gmail.com>
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace WebSocketSharp.Net
{
  internal sealed class EndPointManager
  {
    #region Private Fields

    private static readonly Dictionary<IPEndPoint, EndPointListener> _endpoints;

    #endregion

    #region Static Constructor

    static EndPointManager ()
    {
      _endpoints = new Dictionary<IPEndPoint, EndPointListener> ();
    }

    #endregion

    #region Private Constructors

    private EndPointManager ()
    {
    }

    #endregion

    #region Private Methods

    private static void addPrefix (string uriPrefix, HttpListener listener)
    {
      var pref = new HttpListenerPrefix (uriPrefix);

      var addr = convertToIPAddress (pref.Host);
      if (!addr.IsLocal ())
        throw new HttpListenerException (87, "Includes an invalid host.");

      int port;
      if (!Int32.TryParse (pref.Port, out port))
        throw new HttpListenerException (87, "Includes an invalid port.");

      if (!port.IsPortNumber ())
        throw new HttpListenerException (87, "Includes an invalid port.");

      var path = pref.Path;
      if (path.IndexOf ('%') != -1)
        throw new HttpListenerException (87, "Includes an invalid path.");

      if (path.IndexOf ("//", StringComparison.Ordinal) != -1)
        throw new HttpListenerException (87, "Includes an invalid path.");

      var endpoint = new IPEndPoint (addr, port);

      EndPointListener lsnr;
      if (_endpoints.TryGetValue (endpoint, out lsnr)) {
        if (lsnr.IsSecure ^ pref.IsSecure)
          throw new HttpListenerException (87, "Includes an invalid scheme.");
      }
      else {
        lsnr =
          new EndPointListener (
            endpoint,
            pref.IsSecure,
            listener.CertificateFolderPath,
            listener.SslConfiguration,
            listener.ReuseAddress
          );

        _endpoints.Add (endpoint, lsnr);
      }

      lsnr.AddPrefix (pref, listener);
    }

    private static IPAddress convertToIPAddress (string hostname)
    {
      return hostname == "*" || hostname == "+" ? IPAddress.Any : hostname.ToIPAddress ();
    }

    private static void removePrefix (string uriPrefix, HttpListener listener)
    {
      var pref = new HttpListenerPrefix (uriPrefix);

      var addr = convertToIPAddress (pref.Host);
      if (!addr.IsLocal ())
        return;

      int port;
      if (!Int32.TryParse (pref.Port, out port))
        return;

      if (!port.IsPortNumber ())
        return;

      var path = pref.Path;
      if (path.IndexOf ('%') != -1)
        return;

      if (path.IndexOf ("//", StringComparison.Ordinal) != -1)
        return;

      var endpoint = new IPEndPoint (addr, port);

      EndPointListener lsnr;
      if (!_endpoints.TryGetValue (endpoint, out lsnr))
        return;

      if (lsnr.IsSecure ^ pref.IsSecure)
        return;

      lsnr.RemovePrefix (pref, listener);
    }

    #endregion

    #region Internal Methods

    internal static bool RemoveEndPoint (IPEndPoint endpoint)
    {
      lock (((ICollection) _endpoints).SyncRoot) {
        EndPointListener lsnr;
        if (!_endpoints.TryGetValue (endpoint, out lsnr))
          return false;

        _endpoints.Remove (endpoint);
        lsnr.Close ();

        return true;
      }
    }

    #endregion

    #region Public Methods

    public static void AddListener (HttpListener listener)
    {
      var added = new List<string> ();
      lock (((ICollection) _endpoints).SyncRoot) {
        try {
          foreach (var pref in listener.Prefixes) {
            addPrefix (pref, listener);
            added.Add (pref);
          }
        }
        catch {
          foreach (var pref in added)
            removePrefix (pref, listener);

          throw;
        }
      }
    }

    public static void AddPrefix (string uriPrefix, HttpListener listener)
    {
      lock (((ICollection) _endpoints).SyncRoot)
        addPrefix (uriPrefix, listener);
    }

    public static void RemoveListener (HttpListener listener)
    {
      lock (((ICollection) _endpoints).SyncRoot) {
        foreach (var pref in listener.Prefixes)
          removePrefix (pref, listener);
      }
    }

    public static void RemovePrefix (string uriPrefix, HttpListener listener)
    {
      lock (((ICollection) _endpoints).SyncRoot)
        removePrefix (uriPrefix, listener);
    }

    #endregion
  }
}
