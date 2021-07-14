using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Optinuity.TaskManager.BusinessLogic
{
    /// <summary>
    /// Class contains all settings for the application, settings are stored in web.config file.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public static string ApplicationName
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationName"];
            }
        }

        /// <summary>
        /// Gets the application name abbreviated.
        /// </summary>
        /// <value>
        /// The application name abbreviated.
        /// </value>
        public static string ApplicationNameAbbreviated
        {
            get
            {
                return ConfigurationManager.AppSettings["ApplicationNameAbbreviated"];
            }
        }

        /// <summary>
        /// Gets the help email.
        /// </summary>
        /// <value>
        /// The help email.
        /// </value>
        public static string HelpEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["HelpEmail"];
            }
        }

        /// <summary>
        /// Gets the error emails to.
        /// </summary>
        /// <value>
        /// The error emails to.
        /// </value>
        public static string ErrorEmailsTo
        {
            get
            {
                return ConfigurationManager.AppSettings["ErrorEmailsTo"];
            }
        }

        /// <summary>
        /// Gets the notification email from.
        /// </summary>
        /// <value>
        /// The notification email from.
        /// </value>
        public static string NotificationEmailFrom {
            get {

                return ConfigurationManager.AppSettings["NotificationEmailsFrom"];
            }
        }

        /// <summary>
        /// Gets the help link.
        /// </summary>
        /// <value>
        /// The help link.
        /// </value>
        public static string HelpLink
        {
            get
            {

                return ConfigurationManager.AppSettings["HelpLink"];
            }
        }
        /// <summary>
        /// Gets the help link.
        /// </summary>
        /// <value>
        /// The help link.
        /// </value>
        public static string AnalyticSwitch
        {
            get
            {
                return ConfigurationManager.AppSettings["AnalyticSwitch"];
            }
        }
       
    }
}