using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace Optinuity.TaskManager.BusinessLogic
{
    /// <summary>
    /// Email Extension
    /// </summary>
    public static class EmailExtension
    {
        /// <summary>
        /// Sends the email with template.
        /// </summary>
        /// <param name="mailMessage">The mail message.</param>
        public static void SendEmailWithTemplate(MailMessage mailMessage)
        {
            string template = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
                <html xmlns=""http://www.w3.org/1999/xhtml"" >
                <head>
                     <style type=""text/css"">
                            body {font: normal 12px verdana, tahoma ;padding:0px;margin:0px;}
                            p{margin:10px 0px;}
                            h1{font-size:18px;font-weight:bold;margin:0px 0px; padding 0px 5px;}
                            td{vertical-align:top;}
                            hr{border: 1px solid #000;}
                            div.header{background: #fff;vertical-align:middle;font-size:18px;color:#000;padding:5px;border-bottom:solid 2px #015480; margin-bottom:10px;}
                            div.contentPane{padding:20px;}
                            div.footer{border-top:solid 2px #015480;font-size:10px;padding:10px;}
                            TABLE.tabularData {border-width: 0px;border-spacing: 0px;border-style: none;border-color: #fff;border-collapse: collapse;background-color: white;margin-bottom:1.4em;}
                            TABLE.tabularData TH{white-space:nowrap;border-width: 1px;padding: 4px;border-style: solid;border-color: #A4C3EE;background-color: #DBE8F9;text-align:center; }
                            TABLE.tabularData TD{vertical-align : top;border-width: 1px;padding: 4px;border-style: solid;border-color: #A4C3EE;}
                     </style>
                </head>
                <body>
                    <table width=""100%""><tr>
                    <td style = ""height: 50px; font-family: Arial; font-style: italic;font-size: 14pt;font-weight: bold;text-align: left; color :white;background-color: #015480;vertical-align:middle;margin:0px 0px 3px 0px;padding:3px 5px;"">
                    Capital Charges
                    </td>
                    </tr>
                    </table>
                    <p >&nbsp;</p>
                    <div class=""contentPane"">~BODY~</div>
                    <div class=""footer"" >This is a system generated email please do not reply to this email.</div>
                </body>
                </html>";

            string environment = (Optinuity.Framework.Configuration.Environment == "Production") ? "" : Optinuity.Framework.Configuration.Environment;

            template = template.Replace("~ENVIRONMENT~", environment);


            // do not send emails to actual user from dev and test environment.
            if (environment != "")
            {
                StringBuilder replacementMessage = new StringBuilder("<hr />To email in production will go to <ul>");
                foreach (MailAddress address in mailMessage.To)
                {
                    replacementMessage.AppendFormat("<li>{0}</li>", address);
                }
                replacementMessage.Append("</ul>");


                replacementMessage.Append("CC email in production will go to <ul>");

                foreach (MailAddress address in mailMessage.CC)
                {
                    replacementMessage.AppendFormat("<li>{0}</li>", address);
                }

                replacementMessage.Append("</ul>");

                replacementMessage.Append("BCC email in production will go to <ul>");

                foreach (MailAddress address in mailMessage.Bcc)
                {
                    replacementMessage.AppendFormat("<li>{0}</li>", address);
                }
                replacementMessage.Append("</ul><hr />");
                mailMessage.Body += replacementMessage.ToString();
                mailMessage.CC.Clear();
                mailMessage.To.Clear();
                mailMessage.Bcc.Clear();
                mailMessage.To.Add(new MailAddress(Optinuity.Framework.Security.Application.CurrentIdentity.Email));
            }
            mailMessage.From = new MailAddress(AppSettings.NotificationEmailFrom);
            mailMessage.Body = template.Replace("~BODY~", mailMessage.Body);
            mailMessage.IsBodyHtml = true;
            using (SmtpClient client = new SmtpClient())
            {
                client.Send(mailMessage);
            }
        }
    }
}