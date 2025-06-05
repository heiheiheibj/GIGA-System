using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace GIGA_NEW
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // 应用程序启动时执行的代码
            // 初始化应用程序级别的资源
            Application["SystemName"] = System.Configuration.ConfigurationManager.AppSettings["SystemName"];
            Application["SystemVersion"] = System.Configuration.ConfigurationManager.AppSettings["SystemVersion"];
            Application["CompanyName"] = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
            Application["StartTime"] = DateTime.Now;
            Application["OnlineUsers"] = 0;

            // 记录应用程序启动日志
            LogHelper.WriteLog("应用程序启动", "系统", "INFO");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // 会话启动时执行的代码
            // 增加在线用户数
            Application.Lock();
            Application["OnlineUsers"] = (int)Application["OnlineUsers"] + 1;
            Application.UnLock();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // 在每个请求开始时执行的代码
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // 在尝试对用户进行身份验证时执行的代码
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // 在发生未处理的错误时执行的代码
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                // 记录错误日志
                LogHelper.WriteErrorLog(ex);

                // 清除错误
                Server.ClearError();

                // 重定向到错误页面
                Response.Redirect("~/Error.aspx");
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // 会话结束时执行的代码
            // 减少在线用户数
            Application.Lock();
            Application["OnlineUsers"] = (int)Application["OnlineUsers"] - 1;
            if ((int)Application["OnlineUsers"] < 0)
            {
                Application["OnlineUsers"] = 0;
            }
            Application.UnLock();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // 应用程序关闭时执行的代码
            // 记录应用程序关闭日志
            LogHelper.WriteLog("应用程序关闭", "系统", "INFO");
        }
    }
}
