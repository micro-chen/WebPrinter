using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Timers;
//using System.Threading.Tasks;

using WebSocketSharp;
using Microsoft.AspNet.SignalR;

namespace SmartClient.Web.Common
{


    /// <summary>
    /// 向客户端检测有效性的事件参数
    /// </summary>
    public class UserWebSocketStatusCheckEventArgs : EventArgs
    {
        public UserWebSocketStatusCheckEventArgs(string clientId)
        {
            this._MessageId = Guid.NewGuid().ToString().ToLower();
            this.ClientId = clientId;
        }
        private string _MessageId;

        /// <summary>
        /// 参数唯一标志
        /// </summary>
        public string MessageId
        {
            get
            {
                return _MessageId;
            }
        }

        /// <summary>
        /// 对应的客户端的js对象id
        /// </summary>
        public string ClientId { get; private set; }

    }

    /// <summary>
    /// 客户端用户WebSocket连接对象模型
    /// </summary>
    public class UserWebSocket
    {
        public UserWebSocket(string cliengId, WebSocket socket)
        {
            this.ClientId = cliengId;
            this.Socket = socket;
        }

        /// <summary>
        /// 绑定到的Hub
        /// </summary>
        public Hub BindingHub { get; set; }

        /// <summary>
        /// 对应的客户端的js对象id
        /// </summary>
        public string ClientId { get; private set; }


        private WebSocket _socket;
        /// <summary>
        /// 套接字对象
        /// </summary>

        public WebSocket Socket
        {
            get
            {
                return this._socket;
            }
            private set
            {
                this._socket = value;
            }

        }


        /// <summary>
        /// 上次使用时间
        /// </summary>
        public DateTime LastAccessTime { get; set; }


        /// <summary>
        /// 上次检索客户端的有效性的时间
        /// </summary>
        public DateTime LastQueryClientStatusTime { get; set; }

        /// <summary>
        /// 是否即将被回收
        /// </summary>
        public bool IsWillBeGarbage { get; set; }


        #region 事件


        public event EventHandler<UserWebSocketStatusCheckEventArgs> CallBackForCheckClientStatus;

        #endregion


        #region 方法

        /// <summary>
        /// 复位标志态
        /// </summary>
        public void ResetFlags()
        {
            //发送订阅通知后，标志进入回收状态
            this.IsWillBeGarbage = false;
            this.LastAccessTime = DateTime.Now;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Recycle()
        {
            //清空订阅事件
            this.CallBackForCheckClientStatus = null;
            if (null != this.Socket && this.Socket.ReadyState != WebSocketState.Closed)
            {
                this.Socket.Close();
                this.Socket = null;
            }
            //注销绑定的Hub
            if (null!=this.BindingHub)
            {
                this.BindingHub.Dispose();
                this.BindingHub = null;
            }
        }
        public void OnCallBackForCheckClientStatus()
        {
            //未进入回收任务的对象才能进行检测，否则不进行重复检测
            if (null != this.CallBackForCheckClientStatus&&!this.IsWillBeGarbage)
            {
                //发送订阅通知后，标志进入回收状态
                this.LastQueryClientStatusTime = DateTime.Now;
                this.IsWillBeGarbage = true;


                var args = new UserWebSocketStatusCheckEventArgs(this.ClientId);

                //进入垃圾回收任务
                UserWebSocketResolver.LstOfRecycleTasks.Add(new KeyValuePair<string, string>(this.ClientId, args.MessageId));

                this.CallBackForCheckClientStatus(this, args);
            }
        }



        #endregion
    }


    /// <summary>
    /// 用户客户端WebSocket检索器
    /// </summary>
    public static class UserWebSocketResolver
    {

        /// <summary>
        /// 使用的上限，检索时间，超过这个时间的，会被检索是否应该回收。（秒）
        /// （
        ///  对使用超过一定时间的，进行检测回调，发送一个握手消息，如果不能再次握手，那么销毁这个对象
        /// ）
        /// </summary>
        private const int MAX_LIMIT_TIME = 30;


        /// <summary>
        /// 超时上限，推送到客户端后，在10s内未收到响应的套接字对象，被认为是垃圾可回收对象
        /// </summary>
        private const int MAX_OUT_TIME = 10;




        private static Timer _timer_for_clear_outtime_socket;
        /// <summary>
        /// 用户连接字典
        /// </summary>
        public static Dictionary<string, UserWebSocket> DicOfUserWebSocket;

        /// <summary>
        /// 回收任务列表
        /// </summary>
        public static List<KeyValuePair<string, string>> LstOfRecycleTasks;


        /// <summary>
        /// 静态构造函数
        /// </summary>
        static UserWebSocketResolver()
        {
            DicOfUserWebSocket = new Dictionary<string, UserWebSocket>();
            LstOfRecycleTasks = new List<KeyValuePair<string, string>>();

            //初始化一个定时检索器，对超过限制时间的连接进行清理
            _timer_for_clear_outtime_socket = new Timer(MAX_LIMIT_TIME*1000);
            _timer_for_clear_outtime_socket.Elapsed += (object sender, ElapsedEventArgs e) =>
        {
            //1检索已经超过时间上限的对象
            //2出发事件检测
            var lstOfWillNeedCheckStatusSockets = DicOfUserWebSocket.Where(x => null != x.Value
            && (DateTime.Now.Subtract(x.Value.LastAccessTime).TotalSeconds >= MAX_LIMIT_TIME)
            );

            if (lstOfWillNeedCheckStatusSockets != null && lstOfWillNeedCheckStatusSockets.Count() > 0)
            {
                foreach (var item in lstOfWillNeedCheckStatusSockets)
                {
                    if (!default(KeyValuePair<string, UserWebSocket>).Equals(item) && null != item.Value)
                    {
                        item.Value.OnCallBackForCheckClientStatus();//触发检测有效性的事件
                    }
                }
            }


            //3 检测已经发送检测有效性的对象集合
            var lstOfOutTimeSockets = DicOfUserWebSocket.Where(x => null != x.Value
              && x.Value.IsWillBeGarbage == true
              && (DateTime.Now.Subtract(x.Value.LastQueryClientStatusTime).TotalSeconds >= MAX_OUT_TIME)
            );

            if (lstOfOutTimeSockets != null && lstOfOutTimeSockets.Count() > 0)
            {


                for (int i = DicOfUserWebSocket.Keys.Count - 1; i >= 0; i--)
                {
                    var item = DicOfUserWebSocket.ElementAt(i);
                    if (!default(KeyValuePair<string, UserWebSocket>).Equals(item) && null != item.Value)
                    {
                        item.Value.Recycle();//回收对象
                        DicOfUserWebSocket.Remove(item.Key);
                    }
                }


            }


        };


            _timer_for_clear_outtime_socket.Start();
        }

        /// <summary>
        /// 接收一条消息从客户端
        /// 标志此套接字 仍在使用中
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="messageId"></param>
        public static void OnReceiveMessageFromClient(string clientId, string messageId)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(messageId))
            {
                return;
            }

            var garbagePair = LstOfRecycleTasks.FirstOrDefault(x => x.Key == clientId && x.Value == messageId);
            if (default(KeyValuePair<string, string>).Equals(garbagePair))
            {
                return;
            }

            //此套接字对象 依然还在使用中
            LstOfRecycleTasks.Remove(garbagePair);

            UserWebSocket userSocket = null;
            DicOfUserWebSocket.TryGetValue(garbagePair.Key, out userSocket);
            if (null != userSocket)
            {
                userSocket.ResetFlags();//复位对象
            }
        }

        /// <summary>
        /// 从字典中获取用户的 websocket
        /// </summary>
        /// <param name="cliengId"></param>
        /// <returns></returns>
        public static UserWebSocket GetUserSocketFromDic(string cliengId)
        {
            UserWebSocket socket = null;

            try
            {
                DicOfUserWebSocket.TryGetValue(cliengId, out socket);
                if (null!=socket)
                {
                    socket.LastAccessTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return socket;
        }

        /// <summary>
        /// 添加到websocket 字典中
        /// </summary>
        /// <param name="uSocket"></param>
        /// <returns></returns>
        public static bool AddUserSocketToDic(UserWebSocket uSocket)
        {
            bool result = false;

            try
            {
                if (!DicOfUserWebSocket.ContainsKey(uSocket.ClientId))
                {
                    DicOfUserWebSocket.Add(uSocket.ClientId, uSocket);
                }

                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}