using System;
using System.Reflection;
using log4net;

namespace Demo
{
    class Program
    {
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //var appender = new AliyunLogAppender();
            //appender.Endpoint = ConfigurationManager.AppSettings["AliyunLOG_Endpoint"];
            //appender.AccessKeyId = ConfigurationManager.AppSettings["AliyunLOG_AccessKeyId"];
            //appender.AccessKeySecret = ConfigurationManager.AppSettings["AliyunLOG_AccessKeySecret"];
            //appender.Project = ConfigurationManager.AppSettings["AliyunLOG_Project"];
            //appender.Logstore = ConfigurationManager.AppSettings["AliyunLOG_Logstore"];
            //appender.Topic = ConfigurationManager.AppSettings["AliyunLOG_Topic"];
            //appender.Source = ConfigurationManager.AppSettings["AliyunLOG_Source"];
            //appender.ActivateOptions();

            //BasicConfigurator.Configure(appender);

            _logger.Debug("测试Debug级别");

            _logger.Info("测试Info级别");

            _logger.Error("测试Error级别");

            try
            {
                throw new Exception("系统异常");
            }
            catch (Exception ex)
            {
                _logger.Error("测试Error级别", ex);
            }

            _logger.Debug(new { Message = "测试匿名对象", Propa = "属性a", prop1 = 1, GuidProp = Guid.NewGuid(), DateTimeProp = DateTime.Now});

            _logger.Info(new { Message = "测试匿名对象", Propa = "属性a", prop1 = 1, GuidProp = Guid.NewGuid(), DateTimeProp = DateTime.Now});

            _logger.Error(new { Message = "测试匿名对象", Propa = "属性a", prop1 = 1, GuidProp = Guid.NewGuid(), DateTimeProp = DateTime.Now });

            try
            {
                throw new Exception("系统异常");
            }
            catch (Exception ex)
            {
                _logger.Error(new { Message = "测试匿名对象", Propa = "属性a", prop1 = 1, GuidProp = Guid.NewGuid(), DateTimeProp = DateTime.Now }, ex);
            }
        }
    }
}
