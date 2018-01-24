

#region Contributors
/*
 * Contributors:
 * - Frank Razenberg <frank@zzattack.org>
 */
#endregion

using System;
using System.Runtime.Serialization;

namespace WebSocketSharp
{
    /// <summary>
    /// Represents the event data for the <see cref="WebSocket.OnError"/> event.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   That event occurs when the <see cref="WebSocket"/> gets an error.
    ///   </para>
    ///   <para>
    ///   If you would like to get the error message, you should access
    ///   the <see cref="ErrorEventArgs.Message"/> property.
    ///   </para>
    ///   <para>
    ///   And if the error is due to an exception, you can get it by accessing
    ///   the <see cref="ErrorEventArgs.exception"/> property.
    ///   </para>
    /// </remarks>
    /// 
    [DataContract]
    public class ErrorEventArgs : EventArgs
    {
        #region Private Fields

        private Exception _exception;
        private string _message;

        #endregion

        #region Internal Constructors

        internal ErrorEventArgs(string message)
          : this(message, null)
        {
        }

        internal ErrorEventArgs(string message, Exception exception)
        {
            _message = message;
            _exception = exception;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the exception that caused the error.
        /// </summary>
        /// <value>
        /// An <see cref="System.Exception"/> instance that represents the cause of
        /// the error if it is due to an exception; otherwise, <see langword="null"/>.
        /// </value>
        [DataMember(Name = "exception")]
        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that represents the error message.
        /// </value>
        [DataMember(Name = "message")]
        public string Message
        {
            get
            {
                return _message;
            }
        }

        #endregion
    }
}
