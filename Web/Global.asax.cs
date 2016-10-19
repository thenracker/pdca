using System;
using System.Globalization;
using System.Net.Http.Formatting;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit;

namespace Web
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("json", "true", "application/json"));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var cookie = Request.Cookies["Language"];
            if (cookie?.Value != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cookie.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(cookie.Value);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("cs");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("cs");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
                return;
            try
            {
                new DbErrorDao().Create(new DbError()
                {
                    WindowsId = User.Identity.Name.Trim().ToUpper(),
                    Message = exception.Message,
                    Source = exception.Source,
                    StackTrace = (exception.StackTrace.Length>1000?exception.StackTrace.Substring(0,1000)+" ...":exception.StackTrace),
                    Time = DateTime.Now
                });
            }
            catch
            {
                // ignored
            }
        }
    }
}