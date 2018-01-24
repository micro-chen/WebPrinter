using System;

namespace SmartClient.Model
{
    /// <summary>
    /// 结束的批量打印任务数据包模型
    /// </summary>
    public class TaskModel 
    {
        public TaskModel()
        {
            this.Id = Guid.NewGuid().ToString().ToLower();//32位小写
            this.IsCompressed = true;
            this.CreateTime = DateTime.Now;
        }
        /// <summary>
        /// 任务Id guid 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否已经压缩（默认为true）
        /// </summary>
        public bool IsCompressed { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
