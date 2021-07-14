using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Optinuity.Framework.UI;
using Optinuity.Framework.Diagnostic;
using System.Text.RegularExpressions;
using Optinuity.TaskManager.BusinessLogic;
using System.Reflection;

namespace Optinuity.TaskManager.UI.Controllers
{
    /// <summary>
    /// Responsible for displaying diagnostic screen
    /// </summary>
    public class DiagnosticController : UIControllerBase
    {
        /// <summary>
        /// Action method responsible for displaying diagnostic information
        /// </summary>
        /// <param name="showAll"></param>
        /// <returns></returns>
        public ActionResult Index(bool? showAll)
        {
            DiagnosticItemCollection model = new DiagnosticItemCollection(true);

            // add build version number to the diagnostic information
            model.Add(new DiagnosticItem {
                            GroupName = DiagnosticItemCollection.AppSettingGroup,
                            Name = "BuildNumber",
                            FullValue = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                            Value = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                        });

            PageHeader = "Diagnostic Page For " + AppSettings.ApplicationName;
            PageTitle = "Diagnostic Page";

            // full information is only required when user specifies
            if (showAll == true)
            {
                model.DisplayAll = true;
            }
            else
            {
                model.RemoveExcept(DiagnosticItemCollection.ConectionStringGroup,
                    "Optinuity.TaskManager", "AzManStore");

                model.RemoveExcept(DiagnosticItemCollection.AppSettingGroup,
                    "BuildNumber",
                    "Environment", "ApplicationName", "ApplicationNameAbbreviated", "HelpEmail", "ErrorEmailsTo",
                    "NotificationEmailsFrom", "HelpLink");
            }



            model.Ensure(DiagnosticItemCollection.MailServerSettingGroup,
                "Mail Server", validateMailServer);

            model.Ensure(DiagnosticItemCollection.AppSettingGroup,
                "Environment", validateEnvironment);

            model.Ensure(DiagnosticItemCollection.ConectionStringGroup,
                "Optinuity.TaskManager", validateConnectionString);


            model.Validate();

            return View(model);
        }

        #region Private Test Methods
        // Validation method for connection string
        private void validateConnectionString(DiagnosticItem item)
        {
            string machineName = HttpContext.Server.MachineName;

            if (Regex.IsMatch(machineName, "mbamk-nlbiis[\\d]$", RegexOptions.IgnoreCase) &&
                item.FullValue.ToLower().Contains("test"))
            {
                item.ValidationResult = new ValidationResult
                {
                    Success = false,
                    Message = String.Format("Connection string is pointing to test {0}", machineName)
                };
            }
            else
            {
                item.ValidationResult = new ValidationResult { Success = true };
            }
        }

        // Validation methods to check mail server configuration
        private void validateMailServer(DiagnosticItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Value))
                item.ValidationResult = new ValidationResult { Success = false, Message = "Mail Server is required" };
            else
                item.ValidationResult = new ValidationResult { Success = true };
        }

        // validation method to check environment
        private void validateEnvironment(DiagnosticItem item)
        {
            string machineName = HttpContext.Server.MachineName;

            if (Regex.IsMatch(machineName, "mbamk-nlbiis[\\d]$", RegexOptions.IgnoreCase) &&
                item.FullValue != "Production")
            {
                item.ValidationResult = new ValidationResult
                {
                    Success = false,
                    Message = String.Format("Environment must be set to Production on {0}", machineName)
                };
            }
            else
            {
                item.ValidationResult = new ValidationResult { Success = true };
            }
        }

        #endregion

    }
}
