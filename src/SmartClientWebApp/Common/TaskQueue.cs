
using System;
using System.Collections.Generic;
using System.Linq;
using SmartClient.Model;
using SmartClient.Common;
using SmartClient.Common.Compress;

namespace SmartClient.Web.Common
{

    /// <summary>
    /// 任务消息管理
    /// </summary>
    public class TaskQueueManager
    {
        #region 单例模式


        private static TaskQueueManager current;

        public static TaskQueueManager Current
        {
            get
            {
                if (null == current)
                {
                    current = new TaskQueueManager();
                }
                return current;
            }
        }
        #endregion

        public string GetTaskMessage(string taskId)
        {
            var msg = string.Empty;

            var taskModel = this.GetModelById(taskId);
            if (null == taskModel)
            {
                return msg;
            }
            //开始处理消息
            // 对数据进行解压缩
            if (taskModel.IsCompressed)
            {
                msg = LZString.decompressFromEncodedURIComponent(taskModel.Message);
            }
            else
            {
                msg = taskModel.Message;
            }
            return msg;
        }

        /// <summary>
        /// 将消息添加到任务中,并返回任务号
        /// </summary>
        /// <param name="msg></param>
        /// <returns></returns>
        public string AddMessageToQueue(string msg)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(msg))
            {
                return result;
            }
            try
            {
                var modeOfNewInstance = new TaskModel { Message = msg };
                SingletonList<TaskModel>.Instance.Add(modeOfNewInstance);
                result = modeOfNewInstance.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        /// <summary>
        /// 移除指定的任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool RemoveModelById(string taskId)
        {
            bool result = false;

            if (string.IsNullOrEmpty(taskId))
            {
                return result;
            }
            var allTaskList = SingletonList<TaskModel>.Instance;
            if (null == allTaskList || allTaskList.Count <= 0)
            {
                return result;
            }

            //取出指定id的任务消息
            try
            {
                int i = allTaskList.Count - 1;
                while (i >= 0)
                {
                    var model = allTaskList[i];
                    if (null != model && model.Id == taskId)
                    {
                        allTaskList.RemoveAt(i);
                        break;
                    }
                    i--;
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;

        }

        /// <summary>
        /// 取出任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private TaskModel GetModelById(string taskId)
        {
            TaskModel result = null;

            if (string.IsNullOrEmpty(taskId))
            {
                return result;
            }
            var allTaskList = SingletonList<TaskModel>.Instance;
            if (null == allTaskList || allTaskList.Count <= 0)
            {
                return result;
            }

            //取出指定id的任务消息
            try
            {
                int i = allTaskList.Count - 1;
                while (i >= 0)
                {
                    var model = allTaskList[i];
                    if (null != model && model.Id == taskId)
                    {
                        result = model;
                        break;
                    }
                    i--;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;

        }
    }
}