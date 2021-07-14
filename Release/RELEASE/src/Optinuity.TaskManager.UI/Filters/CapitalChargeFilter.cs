using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Optinuity.TaskManager.DataObjects;
using Optinuity.TaskManager.BusinessLogic;
using Optinuity.TaskManager.UI.Helpers;
using Optinuity.Framework.NHibernate;
using Optinuity.Framework.Utilities;
using NHibernate.Criterion;
using System.Web.Security;

namespace Optinuity.TaskManager.UI.Filters
{
    /// <summary>
    /// Action filter specific to Tickler
    /// </summary>
    public class TickerFilter: IActionFilter
    {
        #region IActionFilter Members

        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            
        }

        /// <summary>
        /// Called before an action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.EmailLink = filterContext.RequestContext.HttpContext.Request.Url.ToString();
        }

        #endregion

        
    }
}