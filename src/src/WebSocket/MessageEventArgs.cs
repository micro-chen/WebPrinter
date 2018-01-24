

using System;
using System.Runtime.Serialization;

namespace WebSocketSharp
{
    /// <summary>
    /// Represents the event data for the <see cref="WebSocket.OnMessage"/> event.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   That event occurs when the <see cref="WebSocket"/> receives
    ///   a message or a ping if the <see cref="WebSocket.EmitOnPing"/>
    ///   property is set to <c>true</c>.
    ///   </para>
    ///   <para>
    ///   If you would like to get the message data, you should access
    ///   the <see cref="Data"/> or <see cref="RawData"/> property.
    ///   </para>
    /// </remarks>
    [DataContract]
    public class MessageEventArgs : EventArgs
    {
        #region Private Fields

        private string _data;
        private bool _dataSet;
        private Opcode _opcode;
        private byte[] _rawData;

        #endregion

        #region Internal Constructors

        internal MessageEventArgs(WebSocketFrame frame)
        {
            _opcode = frame.Opcode;
            _rawData = frame.PayloadData.ApplicationData;
        }

        internal MessageEventArgs(Opcode opcode, byte[] rawData)
        {
            if ((ulong)rawData.LongLength > PayloadData.MaxLength)
                throw new WebSocketException(CloseStatusCode.TooBig);

            _opcode = opcode;
            _rawData = rawData;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the opcode for the message.
        /// </summary>
        /// <value>
        /// <see cref="Opcode.Text"/>, <see cref="Opcode.Binary"/>,
        /// or <see cref="Opcode.Ping"/>.
        /// </value>
        internal Opcode Opcode
        {
            get
            {
                return _opcode;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the message data as a <see cref="string"/>.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that represents the message data if its type is
        /// text or ping and if decoding it to a string has successfully done;
        /// otherwise, <see langword="null"/>.
        /// </value>
        [DataMember(Name = "data")]
        public string Data
        {
            get
            {
                setData();
                return _data;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the message type is binary.
        /// </summary>
        /// <value>
        /// <c>true</c> if the message type is binary; otherwise, <c>false</c>.
        /// </value>
        //[DataMember(Name = "isBinary")]
        [IgnoreDataMember]
        public bool IsBinary
        {
            get
            {
                return _opcode == Opcode.Binary;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the message type is ping.
        /// </summary>
        /// <value>
        /// <c>true</c> if the message type is ping; otherwise, <c>false</c>.
        /// </value>
        //[DataMember(Name = "isPing")]
        [IgnoreDataMember]
        public bool IsPing
        {
            get
            {
                return _opcode == Opcode.Ping;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the message type is text.
        /// </summary>
        /// <value>
        /// <c>true</c> if the message type is text; otherwise, <c>false</c>.
        /// </value>
        //[DataMember(Name = "isText")]
        [IgnoreDataMember]
        public bool IsText
        {
            get
            {
                return _opcode == Opcode.Text;
            }
        }

        /// <summary>
        /// Gets the message data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>
        /// An array of <see cref="byte"/> that represents the message data.
        /// </value>
        //[DataMember(Name = "rawData")]
        [IgnoreDataMember]
        public byte[] RawData
        {
            get
            {
                setData();
                return _rawData;
            }
        }

        #endregion

        #region Private Methods

        private void setData()
        {
            if (_dataSet)
                return;

            if (_opcode == Opcode.Binary)
            {
                _dataSet = true;
                return;
            }

            _data = _rawData.UTF8Decode();
            _dataSet = true;
        }

        #endregion
    }
}
