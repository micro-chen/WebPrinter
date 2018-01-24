
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using WebSocketSharp;
using SmartClient.Web.Common;


namespace SmartClient.Web.ServerHubs
{

    /// <summary>
    /// 自定义消息信息
    /// </summary>
    public class CustomMessage
    {
        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; }
    }

    /// <summary>
    /// 打印Hub-消息总线
    /// </summary>
    public class PrinterHub : Hub
    {

        #region 字段


        /// <summary>
        ///  菜鸟的连接套接字，地址是固定的
        /// </summary>
        private const string CAI_NIAO_SOCKET_ADDRESS = "ws://127.0.0.1:13528";



        /// <summary>
        /// 成功ping通过
        /// </summary>
        private const string SUCCESS_PING_STATUS = "success";

        #endregion


        #region 构造函数



        public PrinterHub()
        {
        }

        #endregion


        #region 私有方法


        /// <summary>
        /// 创建一个Web套接字 并初始化事件注册
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="socketAddr"></param>
        /// <returns></returns>
        private UserWebSocket InitOneCaiNiaoSocket(string clientId, string socketAddr)
        {


            WebSocket socket = null;
            if (string.IsNullOrEmpty(socketAddr))
            {
                socket = new WebSocket(CAI_NIAO_SOCKET_ADDRESS);
            }
            else
            {
                socket = new WebSocket(socketAddr);
            }

            //绑定属性
            socket.ClientBindingId = clientId;

            //注册事件
            socket.OnOpen += Socket_OnOpen;
            socket.OnClose += Socket_OnClose;
            socket.OnMessage += Socket_OnMessage;
            socket.OnError += Socket_OnError;



            var userSocket = new UserWebSocket(clientId, socket);
            userSocket.LastAccessTime = DateTime.Now;

            userSocket.CallBackForCheckClientStatus += UserSocket_CallBackForCheckClientStatus;

            UserWebSocketResolver.AddUserSocketToDic(userSocket);

            
            return userSocket;
        }

        /// <summary>
        /// 用户套接字的检测事件处理
        /// 向所有的订阅者发布消息
        /// 客户端接收消息后，检测是否是自身的消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserSocket_CallBackForCheckClientStatus(object sender, UserWebSocketStatusCheckEventArgs e)
        {
            string messageId = e.MessageId;
            string clientId = e.ClientId;

            Clients.All.onCheckClientStatus(messageId, clientId);
        }

        /// <summary>
        /// 接收来自客户端的消息
        /// 确认自身仍在激活中
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="messageId"></param>
        public void GiveMoneyToHouseOwner(string clientId, string messageId)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(messageId))
            {
                return;
            }

            //通知回收器，这个还再激活使用中
            UserWebSocketResolver.OnReceiveMessageFromClient(clientId, messageId);

        }

        /// <summary>
        /// 错误的时候 触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            var userSocket = sender as WebSocket;
            string clientId = string.Empty;
            if (null != userSocket)
            {
                clientId = userSocket.ClientBindingId;
            }

            Clients.Caller.onerror(clientId,e);
        }

        /// <summary>
        /// 接受到消息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            var userSocket = sender as WebSocket;
            string clientId = string.Empty;
            if (null != userSocket)
            {
                clientId = userSocket.ClientBindingId;
            }

            Clients.Caller.onmessage(clientId,e);
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            var userSocket = sender as WebSocket;
            string clientId = string.Empty;
            if (null != userSocket)
            {
                clientId = userSocket.ClientBindingId;

                this.SetClientStatus(userSocket);
            }

            Clients.Caller.onclose(clientId,e);


        }

        /// <summary>
        /// 打开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Socket_OnOpen(object sender, EventArgs e)
        {
            var userSocket = sender as WebSocket;
            string clientId = string.Empty;
            if (null!=userSocket)
            {
                clientId = userSocket.ClientBindingId;

                this.SetClientStatus(userSocket);
            }

            var msg = new CustomMessage { data = "socket has been open." };

            Clients.Caller.onopen(clientId,msg);
        }

        /// <summary>
        /// 验证客户端的请求Id
        /// </summary>
        /// <param name="clientId"></param>
        private bool ValidateRequest(string clientId)
        {
            bool result = true;
            if (string.IsNullOrEmpty(clientId))
            {
                result = false;
                var msg = new CustomMessage { data = "客户端Id参数不能为空! 参数 : clientId" };
                Clients.Caller.onerror(msg);
            }

            return result;
        }

        /// <summary>
        /// 设定客户端的信息
        /// </summary>
        /// <param name="userSocket"></param>
        private void SetClientStatus(WebSocket userSocket)
        {
            var status = (int)userSocket.ReadyState;
            string protocol = userSocket.Protocol;
            Clients.Caller.setClientSocketDescription(status, protocol);
        }

        #endregion


        #region 公开方法

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="taskId"></param>
        public void SendTask(string clientId, string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                return;
            }
            try
            {
                //根据任务号 从队列里取出消息内容 
                string messageFromTaskQueue = TaskQueueManager.Current.GetTaskMessage(taskId);

                ProcessClientMessage(clientId, null, messageFromTaskQueue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //最后不管处理结果如何 都要吧对应编号的任务从队列里删除掉
                TaskQueueManager.Current.RemoveModelById(taskId);
            }
         
        }
        
        /// <summary>
         ///  服务端的Send方法
         /// </summary>
         /// <param name="clientId"></param>
         /// <param name="socketAddr"></param>
         /// <param name="message"></param>
        public void Send(string clientId, string socketAddr, string message)
        {

            ProcessClientMessage(clientId, socketAddr, message);

        }

        /// <summary>
        /// 处理客户端的消息
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="socketAddr"></param>
        /// <param name="message"></param>
        private void ProcessClientMessage(string clientId, string socketAddr, string message)
        {
            if (this.ValidateRequest(clientId) == false)
            {
                return;
            }
            var userSocket = UserWebSocketResolver.GetUserSocketFromDic(clientId);

            if (null == userSocket)
            {
                userSocket = InitOneCaiNiaoSocket(clientId, socketAddr);
            }


            if (userSocket.Socket.ReadyState != WebSocketState.Open)
            {


                try
                {
                    userSocket.Socket.Connect();//连接到指定的套接字接口

                }
                catch (Exception ex)
                {
                    var msg = new CustomMessage { data = string.Format("未能正确连接到Socket连接对象！套接字连接地址：{0}.错误信息：{2}.", userSocket.Socket.Url, ex.Message) };
                    Clients.Caller.onerror(msg);
                    return;
                }
            }





            try
            {
                userSocket.Socket.Send(message);
            }
            catch (Exception ex)
            {

                var msg = new CustomMessage { data = "未能正确发送消息到指定的Socket连接对象！错误信息：" + ex.ToString() };
                Clients.Caller.onerror(msg);
            }
        }

        /// <summary>
        /// 接收一次Ping请求，用来加速客户端与server的握手
        /// </summary>
        /// <param name="clientId"></param>
        public void Ping(string clientId, string socketAddr)
        {
            var data = new CustomMessage { data = SUCCESS_PING_STATUS };
            Clients.Caller.callbackOfPing(data);

            //为此用户创建一个套接字请求
            Task.Factory.StartNew(() =>
            {
                this.InitOneCaiNiaoSocket(clientId, socketAddr);

            });
        }


        #endregion

        #region Dispose


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion




    }
}