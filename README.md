# Aliyun.LOGSDK.log4net Appender
- NuGet：[![Aliyun.LOGSDK.log4net][1.1]][1.2]

[1.1]: https://buildstats.info/nuget/Aliyun.LOGSDK.log4net
[1.2]: https://www.nuget.org/packages/Aliyun.LOGSDK.log4net

使用 Aliyun.LOGSDK.log4net Appender，您可以控制日志的输出目的地为阿里云日志服务，有一点需要特别注意，Aliyun.LOGSDK.log4net Appender 不支持设置日志的输出格式。

# Aliyun.LOGSDK.log4net Appender 的优势

* 客户端日志不落盘：即数据生产后直接通过网络发往服务端。
* 对于已经使用 log4net 记录日志的应用，只需要简单修改配置文件就可以将日志传输到日志服务。

# 使用方法

Step 1： 站点或服务工程中引入依赖。
```
Install-Package Aliyun.LOGSDK.log4net
```

Step 2: 配置 log4net.config 文件（不存在则在项目根目录创建），示例配置文件：
```
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
  </configSections>
  <log4net>
    <!-- Define some output appenders -->
    <!--定义推送到阿里云日志服务中-->
    <appender name="AliyunLogAppender" type="Aliyun.LOGSDK.log4net.AliyunLogAppender, Aliyun.LOGSDK.log4net">
      <!--此处可为空，为空时，从应用程序配置的 AppSettings 节点获取配置项值，其配置 Key 前缀为【AliyunLOG_】-->
      <!--创建 project 所属区域匹配的日志服务 Endpoint，必填参数-->
      <Endpoint value="" />
      <!--阿里云访问秘钥 AccessKeyId，必填参数-->
      <AccessKeyId value="" />
      <!--阿里云访问秘钥 AccessKeySecret，必填参数-->
      <AccessKeySecret value="" />
      <!--日志服务的 project 名，必填参数-->
      <Project value="" />
      <!--日志服务的 logstore 名，必填参数-->
      <Logstore value="" />
      <!--日志主题，必填参数-->
      <Topic value="" />
      <!--日志来源，选填参数-->
      <Source value="" />
      <filter type="log4net.Filter.LevelRangeFilter">
        <param name="LevelMin" value="ALL" />
      </filter>
    </appender>

    <!--定义日志的输出媒介，下面定义日志以五种方式输出。也可以下面的按照一种类型或其他类型输出。-->
    <root>
      <!--control log level: ALL|DEBUG|INFO|WARN|ERROR|FATAL|OFF-->
      <!--如果没有定义LEVEL的值，则缺省为DEBUG-->
      <level value="ALL"/>
      <!--文件形式记录日志-->
      <appender-ref ref="AllAppender"/>
      <!--控制台显示日志-->
      <appender-ref ref="ConsoleAppender"/>
      <!--日志推送到阿里云日志服务，level: ALL-->
      <appender-ref ref="AliyunLogAppender"/>
    </root>
  </log4net>
</configuration>
```

文件 AssemblyInfo.cs 最后添加一行，配置 log4net
```
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
```

也可在 App.config 或 Web.config 配置，当 log4net.config 中配置值为空时，配置项起效。
```
  <appSettings>
    <!--阿里云日志服务配置项，当 log4net.config 中配置值为空时，配置项起效-->
    <!--创建 project 所属区域匹配的日志服务 Endpoint，必填参数-->
    <add key="AliyunLOG_Endpoint" value="cn-beijing.log.aliyuncs.com"/>
    <!--阿里云访问秘钥 AccessKeyId，必填参数-->
    <add key="AliyunLOG_AccessKeyId" value="AccessKeyId"/>
    <!--阿里云访问秘钥 AccessKeySecret，必填参数-->
    <add key="AliyunLOG_AccessKeySecret" value="AccessKeySecret"/>
    <!--日志服务的 project 名，必填参数-->
    <add key="AliyunLOG_Project" value="Project"/>
    <!--日志服务的 logstore 名，必填参数-->
    <add key="AliyunLOG_Logstore" value="Logstore"/>
    <!--日志主题，必填参数-->
    <add key="AliyunLOG_Topic" value="Topic"/>
    <!--日志来源，选填参数-->
    <add key="AliyunLOG_Source" value="Source"/>
  </appSettings>

```

Step 3: 调用示例
```
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
```

写到日志服务中的日志的样式如下：
```
__topic__: 日志主题  
__source__: 日志来源
__Time__: 2017-09-27 16:57:14:002  
__Level__: ERROR
__Logger__: Demo.Program
__Class__: Demo.Program
__Method__: Main
__File__: C:\GitRepos\aliyun-log-log4net-appender\Demo\Program.cs
__Line__: 48
__Thread__: 1
__Message__: 测试匿名对象
__Exception__:
 System.Exception: 系统异常
   在 Demo.Program.Main(String[] args) 位置 C:\GitRepos\aliyun-log-log4net-appender\Demo\Program.cs:行号 48
__ExceptionMessage__: 系统异常
__ExceptionType__: System.Exception

Propa: 属性a
prop1: 1
GuidProp: 5291f321-30ca-4290-b305-01f8d662e9a7
DateTimeProp: 2017/9/27 16:57:14
```

# Loghub Log4j Appender
- [Loghub Log4j Appender](https://github.com/aliyun/aliyun-log-log4j-appender)