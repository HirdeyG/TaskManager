using Newtonsoft.Json;
using Optinuity.TaskManager.UI.Helpers;
using Optinuity.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;
using System.IdentityModel.Claims;

namespace Optinuity.TaskManager.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            log4net.Config.XmlConfigurator.Configure();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
        }

        protected void Application_Error(object sender, EventArgs e)
        {

            try
            {
                Exception lastException = Server.GetLastError();
                log4net.ILog logger = log4net.LogManager.GetLogger("TaskManager");
                logger.Error(lastException);

                UIHelper.SendErrorMessage(HttpContext.Current, lastException);

            }
            catch
            {
                // don't put any thing in catch
            }

        }
    }

    
}