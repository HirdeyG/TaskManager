using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Optinuity.TaskManager.DataObjects;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Routing;
using NHibernate;
using Optinuity.Framework.NHibernate;
using Optinuity.TaskManager.BusinessLogic;
using NHibernate.Criterion;
using System.Web.Security;
using System.Configuration;
using System.Net.Mail;
using Optinuity.TaskManager.UI.ViewModels;

namespace Optinuity.TaskManager.UI.Helpers
{
    /// <summary>
    /// UI helper methods
    /// </summary>
    public static class UIHelper
    {
        

        /// <summary>
        /// Render html attribues from dynamic object
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static string RenderAttributes(this HtmlHelper htmlHelper,object htmlAttributes) {
            return RenderAttributes(htmlAttributes);
        }

        /// <summary>
        /// Render html attribues from dictionary
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static string RenderAttributes(this HtmlHelper htmlHelper, Dictionary<string, object> htmlAttributes)
        {
            return RenderAttributes(htmlAttributes);
        }


        /// <summary>
        /// Gets the selected item text.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="listItem">The list item.</param>
        /// <param name="selectedItem">The selected item.</param>
        /// <returns></returns>
        public static string GetSelectedItemText(this HtmlHelper htmlHelper, List<SelectListItem> listItem,string selectedItem)
        {
            return listItem.Where(r => r.Value == selectedItem).Select(r => r.Text).FirstOrDefault();
        }

        /// <summary>
        /// Render Html Attributes
        /// </summary>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        private static string RenderAttributes(object htmlAttributes)
        {
            StringBuilder rVal = new StringBuilder();

            if (htmlAttributes != null)
            {
                RouteValueDictionary attributes = new RouteValueDictionary(htmlAttributes);

                foreach (string key in attributes.Keys)
                {
                    rVal.AppendFormat("{0}={1} ", key, attributes[key]);
                }
            }

            return rVal.ToString();
        }

        /// <summary>
        /// Renders HTML attributes
        /// </summary>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        private static string RenderAttributes(Dictionary<string, object> htmlAttributes)
        {
            StringBuilder rVal = new StringBuilder();

            if (htmlAttributes != null)
            {
                RouteValueDictionary attributes = new RouteValueDictionary(htmlAttributes);

                foreach (string key in attributes.Keys)
                {
                    rVal.AppendFormat("{0}={1} ", key, attributes[key]);
                }
            }

            return rVal.ToString();
        }

        /// <summary>
        /// Renders select attribute
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static string RenderSelecedAttribute(this HtmlHelper htmlHelper, bool selected)
        {
            if (selected) {
                return "selected=\"selected\"";
            }

            return "";
        }

        /// <summary>
        /// Sends error email to user defined in web.config file.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public static void SendErrorMessage(HttpContext context, Exception exception)
        {
            try
            {
                if (!String.IsNullOrEmpty(AppSettings.ErrorEmailsTo))
                {

                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(
                        AppSettings.NotificationEmailFrom, AppSettings.ErrorEmailsTo);


                    string messageText = exception.Message;
                    string messageTrace = exception.StackTrace;
                    string serverName = context.Server.MachineName;


                    message.Subject = AppSettings.ApplicationName + " " + Optinuity.Framework.Configuration.Environment;
                    message.Body = "User: " + context.User.Identity.Name + "\n" +
                                    "Server: " + serverName + "\n" +
                                    "URL: " + context.Request.Url.ToString() + "\n" +
                                    "Message: " + messageText + "\n" +
                                    "Details: " + exception.ToString();

                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

                    client.Send(message);
                }

            }
            catch
            {
                // don't put any thing in catch
            }

        }

        /// <summary>
        /// Email type enum
        /// </summary>
        public enum EmailType
        {
            NewTask,
            UserAdded,
            UserRemoved,
            OwnerChanged
        };

        /// <summary>
        /// This methiod will send the email
        /// </summary>
        /// <param name="session"></param>
        /// <param name="emailIds"></param>
        /// <param name="ownerEmailId"></param>
        /// <param name="emailType"></param>
        public static void SendEmail(ISession session, List<string> emailIds, long ownerEmailId, EmailType emailType, string taskDefId, string taskName, string loggedInUserEmailId)
        {
            string taskManagerUrl = string.Format("<a href={0}{1}>here</a>",ConfigurationManager.AppSettings["TaskDefinitionUrl"].ToString(), taskDefId);
            string emailBody = string.Format(ConfigurationManager.AppSettings["EmailContentFor" + emailType.ToString()].ToString(), taskManagerUrl, taskName);
            string emailSubject = "Notification from " + AppSettings.ApplicationName;
            SendEmail(GetUserAttribute(session, emailIds, null, UserAttribute.EmailId), GetUserAttribute(session, null, ownerEmailId, UserAttribute.EmailId), emailSubject, emailBody, loggedInUserEmailId);
        }
        /// <summary>
        /// Use for sending emails
        /// </summary>
        /// <param name="model"></param>
        /// <param name="loggedInUserEmailId"></param>
        /// <param name="currentUserFullName"></param>
        public static void SendEmail(TaskVM model, string loggedInUserEmailId, string currentUserFullName)
        {
            //loggedInUserEmailId = "chatta@mbia.com";
            string taskManagerUrl = string.Format("<a href={0}{1}>here</a>", ConfigurationManager.AppSettings["TaskUrl"].ToString(), model.OriginalData.Oid.ToString());
            //string emailBody = string.Format(ConfigurationManager.AppSettings["EmailContentForTaskBeingComplete"].ToString(), model.OriginalData.TaskDefinition.Name, model.OriginalData.TaskStatus.ToString().ToLower(), model.OriginalData.LastUpdatedBy.FullName, taskManagerUrl);
            string emailBody = string.Format(ConfigurationManager.AppSettings["EmailContentForTaskBeingComplete"].ToString(), model.OriginalData.TaskDefinition.Name, model.OriginalData.TaskStatus.ToString().ToLower(), currentUserFullName, taskManagerUrl);
            StringBuilder sb = new StringBuilder();
            foreach (var x in model.NotificationList)
                sb.Append(x.Email + ",");
            string emailSubject = string.Format("Task '{0}' has been {1} ", model.OriginalData.TaskDefinition.Name, model.OriginalData.TaskStatus.ToString().ToLower());
            SendEmail(sb.ToString().Substring(0, sb.Length - 1), model.OriginalData.TaskDefinition.Owner.Email, emailSubject, emailBody, loggedInUserEmailId);
        }

        public enum UserAttribute{
            EmailId,
            FullName
        }
        /// <summary>
        /// This method will get the email ids from the database
        /// </summary>
        /// <param name="session"></param>
        /// <param name="emailIds"></param>
        /// <returns></returns>
        public static string GetUserAttribute(ISession session, List<string> emailIds, long? userId, UserAttribute userAttribute)
        {
            StringBuilder sb = new StringBuilder();
            IList<Person> members = new List<Person>();
            if (emailIds != null)
            {
                members = session.CreateCriteria<Person>()
                    .Add(Expression.In("Oid", emailIds))
                    .List<Person>();
            }
            if (userId != null && userId != 0)
            {
                members = session.CreateCriteria<Person>()
                    .Add(Expression.Eq("Oid", userId))
                    .List<Person>();
            }

            foreach (var member in members)
            {
                if (userAttribute == UserAttribute.EmailId)
                    sb.Append(member.Email + ",");
                else
                    sb.Append(member.FullName + ",");
            }

            if (sb.Length > 1)
                return sb.Remove(sb.Length - 1, 1).ToString();
            return "";
        }

        /// <summary>
        /// This is the actual method that is sending all the emails 
        /// </summary>
        /// <param name="emailTo"></param>
        /// <param name="emailcc"></param>
        /// <param name="body"></param>
        static void SendEmail(string emailTo,string emailcc, string subject, string body, string loggedInUserEmailId)
        {
            try
            {
                string emailToTemp = emailTo;
                if (emailTo.Length == 0)
                    return;
                
                if (Optinuity.Framework.Configuration.Environment != "Production")
                    emailTo = emailcc = loggedInUserEmailId;
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(
                    AppSettings.NotificationEmailFrom, emailTo);
                MailAddress copy = new MailAddress(emailcc);
                message.CC.Add(copy);

                message.IsBodyHtml = true;

                message.Subject = subject;
                if (Optinuity.Framework.Configuration.Environment != "Production")
                    message.Subject = subject + " " + Optinuity.Framework.Configuration.Environment;
                
                if (Optinuity.Framework.Configuration.Environment != "Production")
                {
                    message.Body =getEmailBody( body + "\n" +
                        "In production this email will be send to :" +
                        emailToTemp);
                }
                else
                    message.Body = getEmailBody(body);

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

                client.Send(message);
            }
            catch (Exception ex)
            {
                string ex1 = ex.Message;
                // don't put any thing in catch
            }
        }
        
        /// <summary>
        /// Generating the email body
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string getEmailBody(string content)
        {
            StringBuilder body = new StringBuilder();
            body.Append(getHeader());
            body.Append("<body>");
            body.Append("<h2 class='header' >Task Manager </h2>");
            body.Append(content );
            //body.Append("<p style='padding:2px'>" + content + "</p>");
            body.Append("</body>");
            return body.ToString();
        }

        /// <summary>
        /// Generating the email header
        /// </summary>
        /// <returns></returns>
        private static string getHeader()
        {
            StringBuilder head = new StringBuilder();
            head.Append("<head>");
            head.Append("<style type='text/css'>");
            head.Append("body, TABLE {font-size:85%;color:#000;background:#fff;font-family:verdana,tahoma;} ");
            head.Append(".header { background: #015480;vertical-align:top;color:#fff;color:#fff;font-family: tahoma;} ");
            //head.Append("P {  padding-left: 2cm;}");
            //head.Append("h1 {font-size:2.5em;line-height:1;margin-bottom:0.2em;margin-top:0.2em;}");
            head.Append("TABLE {font-size:100%; border-width: 0px;border-spacing: 0px;border-style: none;border-color: #fff;border-collapse: collapse;background-color: white;margin-bottom:0.6em;}");
            head.Append("TABLE thead {display:table-header-group;}");
            head.Append("TABLE TR {BACKGROUND-COLOR: white;}");
            head.Append("TABLE TH {white-space:nowrap;border-width: 1px;padding: 4px;border-style: solid;border-color: #A4C3EE;background-color: #DBE8F9;text-align:center; vertical-align:bottom;}");
            head.Append("TABLE TD{vertical-align : top;border-width: 1px;padding: 4px;border-style: solid;border-color: #A4C3EE;}");
            head.Append("TABLE TD.noBorder {border:0px #fff;}");
            head.Append("</style>");
            head.Append("</head>");

            return head.ToString();

        }

        /// <summary>
        /// Applies large in clause
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static ICriteria ApplyLargeInClause(this ICriteria criteria, string propertyName,
            long[] values)
        {
            if (values.Count() > 1000)
            {
                List<List<object>> batches = new List<List<object>>();
                batches.Add(new List<object>());

                int counter = 1;
                int batchCount = 0;

                foreach (object obj in values)
                {

                    batches[batchCount].Add(obj);
                    counter++;

                    if (counter == 1000)
                    {
                        counter = 1;
                        batches.Add(new List<object>());
                        batchCount++;
                    }
                }

                Disjunction disjunction = Restrictions.Disjunction();
                foreach (List<object> batch in batches)
                {
                    disjunction.Add(Expression.In(propertyName, batch.ToArray()));
                }

                criteria.Add(disjunction);
            }
            else
            {
                criteria.Add(Expression.In(propertyName, values));
            }


            return criteria;
        }

        /// <summary>
        /// Gets Email For People In Role
        /// </summary>
        /// <param name="session"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static string[] GetEmailForPeopleInRole(ISession session, string roleName)
        {
            List<string> rVal = new List<string>();

            string[] users = Roles.GetUsersInRole(roleName);
            string[] userLogins = users.Select(r => r.Split('@')[0].ToLower()).ToArray();

            IList<Person> members = session.CreateCriteria<Person>()
                .Add(Expression.In("LoginName", userLogins))
                .AddOrder(new Order("LastName", true))
                .List<Person>();

            string[] found = members.Select(r=>r.LoginName).ToArray();
            string[] notFound = userLogins.Except(found).ToArray();

            rVal.AddRange(members.Where(r=>r.TerminationDate == null).Select(p=>p.Email));

            return rVal.ToArray();
        }

        /// <summary>
        /// Gets the amount format.
        /// </summary>
        /// <value>
        /// The amount format.
        /// </value>
        public static string AmountFormat { get { return "{0:$#,##0;($#,##0)}"; } }

        /// <summary>
        /// Gets the amount format in million.
        /// </summary>
        /// <value>
        /// The amount format in million.
        /// </value>
        public static string AmountFormatInMillion { get { return "{0:$#,##0,,.00;($#,##0,,.00)}"; } }

        /// <summary>
        /// Executes the partial view.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static string ExecutePartialView(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.ToString();
            }
        }


    }
}