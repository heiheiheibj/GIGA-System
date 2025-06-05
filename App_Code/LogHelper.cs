using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// LogHelper 日志辅助类
/// </summary>
public class LogHelper
{
    // 日志文件路径
    private static readonly string logPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LogPath"]);
    // 是否启用调试日志
    private static readonly bool enableDebugLog = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDebugLog"]);
    // 是否启用错误日志
    private static readonly bool enableErrorLog = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableErrorLog"]);
    // 是否启用操作日志
    private static readonly bool enableOperationLog = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableOperationLog"]);

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="module">模块名称</param>
    /// <param name="logType">日志类型（INFO、DEBUG、ERROR、WARN）</param>
    public static void WriteLog(string message, string module, string logType)
    {
        // 根据日志类型判断是否需要记录
        if (logType == "DEBUG" && !enableDebugLog) return;
        if (logType == "ERROR" && !enableErrorLog) return;
        if ((logType == "INFO" || logType == "WARN") && !enableOperationLog) return;

        try
        {
            // 确保日志目录存在
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            // 构建日志文件名（按日期分文件）
            string fileName = $"{logPath}\\{DateTime.Now.ToString("yyyy-MM-dd")}.log";

            // 构建日志内容
            string logContent = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] [{logType}] [{module}] {message}";

            // 获取当前用户信息
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["UserName"] != null)
            {
                string userName = HttpContext.Current.Session["UserName"].ToString();
                logContent += $" [User: {userName}]";
            }

            // 获取客户端IP
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                string clientIP = HttpContext.Current.Request.UserHostAddress;
                logContent += $" [IP: {clientIP}]";
            }

            // 写入日志文件
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(logContent);
                sw.Flush();
                sw.Close();
            }
        }
        catch
        {
            // 日志记录失败，不做处理
        }
    }

    /// <summary>
    /// 写入错误日志
    /// </summary>
    /// <param name="ex">异常对象</param>
    public static void WriteErrorLog(Exception ex)
    {
        if (!enableErrorLog) return;

        try
        {
            // 确保日志目录存在
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            // 构建日志文件名（按日期分文件）
            string fileName = $"{logPath}\\{DateTime.Now.ToString("yyyy-MM-dd")}_error.log";

            // 构建日志内容
            string logContent = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] [ERROR] 异常信息：{ex.Message}\r\n";
            logContent += $"异常类型：{ex.GetType().FullName}\r\n";
            logContent += $"堆栈跟踪：{ex.StackTrace}\r\n";

            // 记录内部异常
            if (ex.InnerException != null)
            {
                logContent += $"内部异常：{ex.InnerException.Message}\r\n";
                logContent += $"内部异常堆栈：{ex.InnerException.StackTrace}\r\n";
            }

            // 获取当前请求信息
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request != null)
                {
                    logContent += $"请求URL：{HttpContext.Current.Request.Url}\r\n";
                    logContent += $"请求方法：{HttpContext.Current.Request.HttpMethod}\r\n";
                    logContent += $"客户端IP：{HttpContext.Current.Request.UserHostAddress}\r\n";
                    logContent += $"用户代理：{HttpContext.Current.Request.UserAgent}\r\n";
                }

                if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserName"] != null)
                {
                    logContent += $"当前用户：{HttpContext.Current.Session["UserName"]}\r\n";
                }
            }

            logContent += "----------------------------------------\r\n";

            // 写入日志文件
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(logContent);
                sw.Flush();
                sw.Close();
            }
        }
        catch
        {
            // 日志记录失败，不做处理
        }
    }

    /// <summary>
    /// 写入调试日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="module">模块名称</param>
    public static void WriteDebugLog(string message, string module)
    {
        WriteLog(message, module, "DEBUG");
    }

    /// <summary>
    /// 写入信息日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="module">模块名称</param>
    public static void WriteInfoLog(string message, string module)
    {
        WriteLog(message, module, "INFO");
    }

    /// <summary>
    /// 写入警告日志
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <param name="module">模块名称</param>
    public static void WriteWarnLog(string message, string module)
    {
        WriteLog(message, module, "WARN");
    }

    /// <summary>
    /// 写入操作日志
    /// </summary>
    /// <param name="operationType">操作类型</param>
    /// <param name="operationContent">操作内容</param>
    /// <param name="module">模块名称</param>
    public static void WriteOperationLog(string operationType, string operationContent, string module)
    {
        string message = $"操作类型：{operationType}，操作内容：{operationContent}";
        WriteLog(message, module, "INFO");
    }
}
