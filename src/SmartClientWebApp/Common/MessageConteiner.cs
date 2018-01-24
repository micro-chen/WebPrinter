using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartClient.Web
{

    /// <summary>
    /// 消息容器接口-契约
    /// </summary>
    public interface IMessageConteiner
    {
        MessageStatus Status { get; set; }
        string Message { get; set; }
    }



    /// <summary>
    /// API交互的容器
    /// </summary>
    public class MessageConteiner<T>:IMessageConteiner
    {
        public MessageConteiner()
        {
            this.Status = MessageStatus.Success;
        }
        /// <summary>
        /// 结果状态
        /// </summary>
        public MessageStatus Status { get; set; }

        private string _Message = "";
        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }


        private T _Data;
        /// <summary>
        /// 数据
        /// </summary>
        public T Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

    }
    public enum MessageStatus
    {
        /// <summary>
        /// 失败
        /// </summary>
        Error = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1
    }
}