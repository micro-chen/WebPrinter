

using System;
using System.Runtime.Serialization;

namespace WebSocketSharp
{
    /// <summary>
    /// Represents the event data for the <see cref="WebSocket.OnClose"/> event.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   That event occurs when the WebSocket connection has been closed.
    ///   </para>
    ///   <para>
    ///   If you would like to get the reason for the close, you should access
    ///   the <see cref="Code"/> or <see cref="Reason"/> property.
    ///   </para>
    /// </remarks>
   [DataContract]
    public class CloseEventArgs : EventArgs
    {
        #region Private Fields

        private bool _clean;
        private PayloadData _payloadData;

        #endregion

        #region Internal Constructors

        internal CloseEventArgs()
        {
            _payloadData = PayloadData.Empty;
        }

        internal CloseEventArgs(ushort code)
          : this(code, null)
        {
        }

        internal CloseEventArgs(CloseStatusCode code)
          : this((ushort)code, null)
        {
        }

        internal CloseEventArgs(PayloadData payloadData)
        {
            _payloadData = payloadData;
        }

        internal CloseEventArgs(ushort code, string reason)
        {
            _payloadData = new PayloadData(code, reason);
        }

        internal CloseEventArgs(CloseStatusCode code, string reason)
          : this((ushort)code, reason)
        {
        }

        #endregion

        #region Internal Properties

        internal PayloadData PayloadData
        {
            get
            {
                return _payloadData;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the status code for the close.
        /// </summary>
        /// <value>
        /// A <see cref="ushort"/> that represents the status code for the close if any.
        /// </value>
        [DataMember(Name = "code")]
        public ushort Code
        {
            get
            {
                return _payloadData.Code;
            }
        }

        /// <summary>
        /// Gets the reason for the close.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that represents the reason for the close if any.
        /// </value>
        [DataMember(Name = "reason")]
        public string Reason
        {
            get
            {
                return _payloadData.Reason ?? String.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connection has been closed cleanly.
        /// </summary>
        /// <value>
        /// <c>true</c> if the connection has been closed cleanly; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "wasClean")]
        public bool WasClean
        {
            get
            {
                return _clean;
            }

            internal set
            {
                _clean = value;
            }
        }

        #endregion
    }
}
