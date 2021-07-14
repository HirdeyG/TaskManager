using System.Web;
using System.Web.Mvc;

namespace Optinuity.TaskManager.UI
{
    /// <summary>
    /// Filter Config
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // filter for nhibernate transaction
            filters.Add(new Optinuity.TaskManager.UI.Filters.NHibernateTransactionFilter(), 0);

            // filer for ticker
            filters.Add(new Optinuity.TaskManager.UI.Filters.TickerFilter(), 1);

            // filter for navigation
            filters.Add(new Optinuity.Framework.UI.NavigationActionFilter(), 2);

        }
    }
}