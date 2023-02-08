using System;
using System.Collections.Generic;
using System.Configuration;
using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Data;
using Aliyun.Api.LOG.Request;
using Aliyun.Api.LOG.Response;
using log4net.Appender;
using log4net.Core;
using log4net.Util;
using Newtonsoft.Json.Linq;

namespace Aliyun.LOGSDK.log4net
{
    /// <summary>
    /// 阿里云日志服务 log4net Appender
    /// </summary>
    public class AliyunLogAppender : AppenderSkeleton
    {
        /// <summary>
        /// 创建 project 所属区域匹配的日志服务 Endpoint
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// 阿里云访问秘钥 AccessKeyId
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// 阿里云访问秘钥 AccessKeySecret
        /// </summary>
        public string AccessKeySecret { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// 日志库名称
        /// </summary>
        public string Logstore { get; set; }

        /// <summary>
        /// 日志主题
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 日志来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 阿里云日志服务客户端实例
        /// </summary>
        private LogClient _client;

        /// <summary>
        /// 初始化日志写入器
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            try
            {
                // 配置项为空时，从应用程序配置的AppSettings节点获取配置项值，其配置Key前缀为【AliyunLOG_】
                if (string.IsNullOrEmpty(Endpoint))
                {
                    Endpoint = ConfigurationManager.AppSettings["AliyunLOG_Endpoint"];
                }
                if (string.IsNullOrEmpty(AccessKeyId))
                {
                    AccessKeyId = ConfigurationManager.AppSettings["AliyunLOG_AccessKeyId"];
                }
                if (string.IsNullOrEmpty(AccessKeySecret))
                {
                    AccessKeySecret = ConfigurationManager.AppSettings["AliyunLOG_AccessKeySecret"];
                }
                if (string.IsNullOrEmpty(Project))
                {
                    Project = ConfigurationManager.AppSettings["AliyunLOG_Project"];
                }
                if (string.IsNullOrEmpty(Logstore))
                {
                    Logstore = ConfigurationManager.AppSettings["AliyunLOG_Logstore"];
                }
                if (string.IsNullOrEmpty(Topic))
                {
                    Topic = ConfigurationManager.AppSettings["AliyunLOG_Topic"];
                }
                if (string.IsNullOrEmpty(Source))
                {
                    Source = ConfigurationManager.AppSettings["AliyunLOG_Source"];
                }

                _client = new LogClient(Endpoint, AccessKeyId, AccessKeySecret);

                LogLog.Debug(this.GetType(), "AliyunLogAppender 初始化完成");
            }
            catch (Exception ex)
            {
                LogLog.Warn(this.GetType(), "AliyunLogAppender 初始化异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 推送日志
        /// </summary>
        /// <param name="loggingEvent">日志信息</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {

                List<LogItem> logs = new List<LogItem>();
                LogItem item = new LogItem();

                DateTime unixTimestampZeroPoint = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                item.Time = (uint) ((DateTime.UtcNow - unixTimestampZeroPoint).TotalSeconds);

                item.PushBack("__Time__", loggingEvent.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                item.PushBack("__Level__", loggingEvent.Level.ToString());
                item.PushBack("__Logger__", loggingEvent.LoggerName);
                item.PushBack("__Class__", loggingEvent.LocationInformation.ClassName);
                item.PushBack("__Method__", loggingEvent.LocationInformation.MethodName);
                item.PushBack("__File__", loggingEvent.LocationInformation.FileName);
                item.PushBack("__Line__", loggingEvent.LocationInformation.LineNumber);
                item.PushBack("__Thread__", loggingEvent.ThreadName);

                if (loggingEvent.MessageObject is string)
                {
                    item.PushBack("__Message__", loggingEvent.MessageObject.ToString());
                }
                else
                {
                    FlattenMessageObject(loggingEvent.MessageObject, item);
                }

                if (loggingEvent.ExceptionObject != null)
                {
                    item.PushBack("__Exception__", loggingEvent.ExceptionObject.ToString());
                    item.PushBack("__ExceptionMessage__", loggingEvent.ExceptionObject.Message);
                    item.PushBack("__ExceptionType__", loggingEvent.ExceptionObject.GetType().FullName);
                }

                var properties = loggingEvent.GetProperties();
                foreach (var key in properties.GetKeys())
                {
                    item.PushBack(key, properties[key].ToString());
                }

                logs.Add(item);

                PutLogsResponse response = _client.PutLogs(new PutLogsRequest(Project, Logstore, Topic, Source, logs));
            }
            catch (Exception ex)
            {
                LogLog.Debug(this.GetType(), "推送日志到阿里云异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 展开消息对象属性
        /// </summary>
        /// <param name="obj">消息对象</param>
        /// <param name="item">日志属性项目列表</param>
        /// <param name="prefix">属性前缀</param>
        private void FlattenMessageObject(object obj, LogItem item, string prefix = "")
        {
            if (obj == null)
            {
                return;
            }

            var jobj = JObject.FromObject(obj);
            foreach (var property in jobj.Properties())
            {
                var jValue = property.Value as JValue;
                if (jValue != null)
                {
                    var key = prefix + property.Name;

                    if (key == "Message")
                    {
                        key = "__Message__";
                    }

                    item.PushBack(key, property.Value != null ? property.Value.ToString() : null);
                }
            }
        }

        /// <summary>
        /// 关闭日志写入器
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();

            _client = null;
        }
    }
}
