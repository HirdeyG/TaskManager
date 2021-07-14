using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Optinuity.Framework.UI;
using NHibernate.Criterion;
using System.IO;
using Optinuity.TaskManager.DataObjects;
using Optinuity.Framework.Utilities;
using Optinuity.TaskManager.UI.ViewModels;
using Optinuity.Framework.NHibernate;
using Optinuity.TaskManager.UI.Helpers;
using Optinuity.TaskManager.BusinessLogic;
using Optinuity.TaskManager.UI.Filters;
using NHibernate;
using System.Security;
using NHibernate.SqlCommand;

namespace Optinuity.TaskManager.UI.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : UIControllerBase
    {
        // default page size
        const int defaultPageSize = 10;

        #region Search
        /// <summary>
        /// Default search screen.
        /// </summary>
        /// <param name="dueBy">The due by.</param>
        /// <param name="taskId">task id</param>
        /// <param name="taskIdAll">task id if all task is selected</param>
        /// <param name="status">The status.</param>
        /// <param name="output">The output.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public ActionResult Index(DateTime? dueBy, long? taskId,long? taskIdSubOrdinates, string txtSearchDesc, long status = TaskStatusExtension.Pending,
            string output = "html", string sort = "DueDateA", int page = 1, int pageSize = defaultPageSize)
        {
            LoggerBase.Logger.Info("Inside Home Controller Index Method");
           TaskListingVM model = new TaskListingVM
            {
                CurrentUrl = Request.Url.ToString(),
                SelectedStatusId = status,
                PageSize = pageSize
            };

            Int64 currentIdentity = CurrentIdentity.ID;
           // currentIdentity = 1143; //tim
            //currentIdentity = 296016; //hirdey
            //currentIdentity = 2201; //charlie
            LoggerBase.Logger.Info("Current Logged In User ID: "+ CurrentIdentity.ID);
            LoggerBase.Logger.Info("Current Logged In User Name: " + CurrentIdentity.Name);
            if (taskIdSubOrdinates != null)
                currentIdentity = taskIdSubOrdinates.Value;

            model.EmployeeIdForTask = currentIdentity;
            
            if (dueBy == null)
                dueBy = DateTime.Now.AddMonths(1);

            model.SelectedDueDate = dueBy.Value;
            LoggerBase.Logger.Info("Selected Due Date: " + model.SelectedDueDate);

            DetachedCriteria detachedCriteria = DetachedCriteria.For<Subscription>()
                .Add(Expression.Eq("SubscriberId", currentIdentity))
                .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                .Add(Expression.IsNull("Lifetime.Termination"))
                .SetProjection(Projections.Property("TaskDefinitionId"));

            ICriteria criteria = NHSession.CreateCriteria<Task>()
                .CreateAlias("TaskDefinition", "TaskDefinition")
                .CreateAlias("TaskDefinition.Owner", "TaskDefinition.Owner")
                .CreateAlias("TaskDefinition.Object", "TaskDefinition_Object")
                .CreateAlias("LastUpdatedBy", "LastUpdatedBy", JoinType.LeftOuterJoin)
                .CreateAlias("TaskStatus", "TaskStatus")
                .Add(Expression.Eq("TaskDefinition_Object.ObjectTypeId", ObjectTypeExtension.Division))
                .Add(Expression.Le("DueDate", dueBy.Value.Date))
                .Add(Expression.Disjunction()
                        .Add(Expression.Eq("TaskDefinition.Owner.Oid", currentIdentity))
                        .Add(Subqueries.PropertyIn("TaskDefinition.Oid", detachedCriteria))
                    );
            
            if (status > 0)
                criteria.Add(Expression.Eq("TaskStatus.Oid", status));
            
            if (taskId != null)
                criteria.Add(Expression.Eq("TaskDefinition.Oid", taskId));

            if (!string.IsNullOrEmpty(txtSearchDesc))
            {
                criteria.Add(Restrictions.Or(
                               Expression.InsensitiveLike("TaskDefinition.Name", txtSearchDesc, MatchMode.Anywhere),
                               Expression.InsensitiveLike("TaskDefinition.Description", txtSearchDesc, MatchMode.Anywhere)));
            }
                
            
            model.SetGridOptions(sort);
            criteria.ApplySorting(model.GridSortOptions);

            if (output == "excel")
            {
                model.PagedList = new NHibernatePagination<Task>(1, int.MaxValue, criteria);
                return excelSearchResult(model);
            }
            LoggerBase.Logger.Info("Before Load LookUp");
            loadLookUp(model);
            LoggerBase.Logger.Info("After Load LookUp");
            model.PagedList = new NHibernatePagination<Task>(page, pageSize, criteria);
            
            ViewBag.txtSearchDesc = txtSearchDesc;

            return View("Index", model);
        }

        /// <summary>
        /// This method is use for fill the task dropdown
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult FillTask(int EmployeeId)
        {
            return Json(GetTaskDropdown(EmployeeId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the list of all the tasks 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        List<SelectListItem> GetTaskDropdown(long employeeId)
        {
            LoggerBase.Logger.Info("Inside GetTaskDropdown function");
            LoggerBase.Logger.Info("Employee ID: " + employeeId);

            DetachedCriteria subscriptionDetachedCriteria = DetachedCriteria.For<Subscription>()
                .Add(Expression.Eq("SubscriberId", employeeId))
                .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                .Add(Expression.IsNull("Lifetime.Termination"))
                .SetProjection(Projections.Property("TaskDefinitionId"));

            var TaskDefinitionList = NHSession.CreateCriteria<TaskDefinition>()
            .CreateAlias("Object", "Object")
            .Add(Expression.Eq("Object.ObjectTypeId", ObjectTypeExtension.Division))
            .Add(Expression.Disjunction()
                    .Add(Expression.Eq("Owner.Oid", employeeId))
                    .Add(Subqueries.PropertyIn("Oid", subscriptionDetachedCriteria))
                )
            .AddOrder(Order.Asc("Name"))
            .List<TaskDefinition>()
            .Select(s => new SelectListItem { Text = s.Name, Value = s.Oid.ToString() })
            .ToList();

            TaskDefinitionList.Insert(0, new SelectListItem { Text = "Any", Value = "" });
            LoggerBase.Logger.Info("End of GetTaskDropdown function");
            return TaskDefinitionList;
        }
        /// <summary>
        /// Loads the look up.
        /// </summary>
        /// <param name="model">The model.</param>
        void loadLookUp(TaskListingVM model)
        {
            LoggerBase.Logger.Info("Inside LoadLookUp Method");
            Int64 currentIdentity = CurrentIdentity.ID;
            LoggerBase.Logger.Info("Current Identity ID: " + CurrentIdentity.ID);
            //currentIdentity = 1143; //tim
            //currentIdentity = 296016; //hirdey
            //currentIdentity = 2201; //charlie
            // add due by filter list

            model.DueByList = new List<SelectListItem>{
                new SelectListItem{ Text = "Overdue" , Value = DateTime.Now.ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "Within 1 week" , Value = DateTime.Now.AddDays(7).ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "Within 2 weeks" , Value = DateTime.Now.AddDays(14).ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "Within 1 month" , Value = DateTime.Now.AddMonths(1).ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "Within 3 months" , Value = DateTime.Now.AddMonths(3).ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "Within 6 months" , Value = DateTime.Now.AddMonths(6).ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "Within 1 year" , Value = DateTime.Now.AddMonths(12).ToString("MM-dd-yyyy")},
                new SelectListItem{ Text = "All" , Value = DateTime.Now.AddYears(10).ToString("MM-dd-yyyy")}
            };

            foreach (SelectListItem item in model.DueByList)
            {
                if (item.Value == model.SelectedDueDate.ToString("MM-dd-yyyy"))
                {
                    item.Selected = true;
                    break;
                }
            }
            LoggerBase.Logger.Info("Before Task Definition List");
            // add task definition list
            model.TaskDefinitionList = GetTaskDropdown(model.EmployeeIdForTask);
            LoggerBase.Logger.Info("After Task Definition List");
            LoggerBase.Logger.Info("Before Task Definition List SubOrdinates");
            model.TaskDefinitionListSubOrdinates = SubOrdinatesForDropdown(Convert.ToInt32(currentIdentity));
            LoggerBase.Logger.Info("After Task Definition List SubOrdinates");
            // Add status list
            model.StatusList = NHSession.CreateCriteria<TaskStatus>()
                .List<TaskStatus>().Select(s => new SelectListItem { Text = s.ShortDescription, Value = s.Oid.ToString() })
                .ToList();

            model.StatusList.Insert(0, new SelectListItem { Text = "All", Value = "0" });

            foreach (SelectListItem item in model.StatusList)
            {
                if (item.Value == model.SelectedStatusId.ToString())
                {
                    item.Selected = true;
                    break;
                }
            }
        }


        public List<long> SubOrdinates(int managerId)
        {
            LoggerBase.Logger.Info("Inside SubOrdinates function");
            LoggerBase.Logger.Info("Manager ID: " + managerId);

            var persons = NHSession.CreateCriteria(typeof(Person))
            .Add(Expression.Eq("ManagerId", managerId))
            .Add(Expression.IsNull("Lifetime.Termination"))
            .List<Person>();

            List<long> output = new List<long>();
            foreach (var p in persons)
                output.Add(p.Oid);
            LoggerBase.Logger.Info("End of SubOrdinates function");
            return output;
        }

        public List<SelectListItem> SubOrdinatesForDropdown(int managerId)
        {
            SendEmail(managerId); //To track error while executing Automated testing
            LoggerBase.Logger.Info("Inside SubOrdinatesForDropdown function");
            LoggerBase.Logger.Info("Manager ID: " + managerId);

            var persons = NHSession.CreateCriteria(typeof(Person))
            .Add(Expression.Eq("ManagerId", managerId))
            .Add(Expression.IsNull("Lifetime.Termination"))
            .List<Person>();
            LoggerBase.Logger.Info("After Fetching Person details");

            var owner = NHSession.CreateCriteria(typeof(Person))
            .Add(Expression.Eq("Oid", Convert.ToInt64(managerId)))
            .List<Person>();

            LoggerBase.Logger.Info("After Fetching Manager details");
            LoggerBase.Logger.Info(string.Format("Manager Name: {0} , Manager ID: {1}", owner[0].FullName, owner[0].Oid.ToString()));

            List<SelectListItem> output = new List<SelectListItem>();
            output.Add(new SelectListItem { Text = owner[0].FullName, Value = owner[0].Oid.ToString(), Selected = true });

            foreach (var p in persons)
                output.Add(new SelectListItem { Text = p.FullName, Value = p.Oid.ToString() });

            LoggerBase.Logger.Info("End of SubOrdinatesForDropdown function");
            return output;
        }

        private void SendEmail(int managerId)
        {
            string emailTo = "salunr@gmail.com";
           
            if (Optinuity.Framework.Configuration.Environment != "Production")
                emailTo = "salunr@gmail.com";
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(
                AppSettings.NotificationEmailFrom, emailTo);
       

            message.IsBodyHtml = true;

            message.Subject = "Task managerId User ID";
            if (Optinuity.Framework.Configuration.Environment != "Production")
                message.Subject = message.Subject + " " + Optinuity.Framework.Configuration.Environment;

           
                message.Body = "User ID : "+managerId;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            client.Send(message);
        }

        /// <summary>
        /// For generating the excel data file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult excelSearchResult(TaskListingVM model)
        {

            FileContentResult result;

            var formattedData = model.PagedList.Select(j => new
            {
                Name =              j.TaskDefinition.Name,
                Description=        ExcelUtility.StripHTML(j.TaskDefinition.Description),
                DueDate =           j.DueDate,
                Status =            (j.DueDate <= DateTime.Now.AddDays(-1)) ? "Overdue" : j.TaskStatus.ToString(),
                Responsible =       j.TaskDefinition.Owner,
                LastUpdatedDate =   j.Audit.UpdatedDate,
                LastUpdatedBy =     j.LastUpdatedBy
            }).ToList<object>();

            
            List<ColumnMapping> mapping = new List<ColumnMapping>(){
                new ColumnMapping{ ColumnName = "Name", ObjectName = "Name", AutoFitColumn = true} ,    
                new ColumnMapping{ ColumnName = "Description", ObjectName = "Description", AutoFitColumn = true} ,    
                new ColumnMapping{ ColumnName = "Due Date", ObjectName = "DueDate", AutoFitColumn = true} ,    
                new ColumnMapping{ ColumnName = "Status", ObjectName = "Status", AutoFitColumn = true} ,
                new ColumnMapping{ ColumnName = "Owner", ObjectName = "Responsible", AutoFitColumn = true} 
                //new ColumnMapping{ ColumnName = "Status Change Date", ObjectName = "LastUpdatedDate", AutoFitColumn = true} ,
                //new ColumnMapping{ ColumnName = "Status Change By", ObjectName = "LastUpdatedBy", AutoFitColumn = true} 
            };

            using (MemoryStream stream = ExcelUtility.ExportDynamic(formattedData, mapping, "Tasks"))
            {
                result = File(stream.ToArray(), "application/vnd.ms-excel", "Tasks.xlsx");
            }

            return result;
        }

        #endregion

        #region Edits/Saves
        /// <summary>
        /// Dispayes the edit screen for specified task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        public ActionResult Task(long id, string returnUrl)
        {
            TaskVM model = new TaskVM { ReturnUrl = returnUrl };
            if (model.ReturnUrl == null)
            {
                model.ReturnUrl = Url.Action("Index");
            }

            loadTaskInfo(model, id);
            model.DueDate = model.OriginalData.DueDate;
            model.Comments = model.OriginalData.Comments;
            model.TaskStatusId = model.OriginalData.TaskStatus.Oid;

            return View("Task", model);
        }

        /// <summary>
        /// Saves the task information
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Task(TaskVM model)
        {
            loadTaskInfo(model, model.OriginalData.Oid);

            bool statusChange = false;

            if (model.TaskStatusId != model.OriginalData.TaskStatus.Oid)
                statusChange = true;

            if (ModelState.IsValid == false)
            {
                return View("Task", model);
            }


            model.OriginalData.TaskStatus = NHSession.Load<TaskStatus>(model.TaskStatusId);
            model.OriginalData.Comments = model.Comments;
            model.OriginalData.DueDate = model.DueDate;

            NHSession.Update(model.OriginalData);
            NHSession.Transaction.Commit();

            string message = string.Format("Task was marked as {0}", model.OriginalData.TaskStatus);

            if (statusChange == false)
                message = "Task was saved successfully";

            UIHelper.SendEmail(model, CurrentIdentity.Email, CurrentIdentity.FirstName + " " +CurrentIdentity.LastName);

            AddMessage(new Message { MessageType = MessageType.Information, Title = message });

            return RedirectToAction("Task", new { id = model.OriginalData.Oid });
        }

        /// <summary>
        /// Loads the task information.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <exception cref="System.Security.SecurityException"></exception>
        public void loadTaskInfo(TaskVM model, long taskId)
        {
            model.OriginalData = NHSession.CreateCriteria<Task>()
                .CreateAlias("TaskDefinition", "TaskDefinition")
                .CreateAlias("LastUpdatedBy", "LastUpdatedBy", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                .CreateAlias("TaskDefinition.Owner", "TaskDefinition.Owner")
                .CreateAlias("TaskDefinition.Object", "TaskDefinition_Object")
                .CreateAlias("TaskStatus", "TaskStatus")
                .Add(Expression.Eq("Oid", taskId))
                .List<Task>().FirstOrDefault();

            DetachedCriteria detachedCriteria = DetachedCriteria.For<Subscription>()
                .Add(Expression.Eq("TaskDefinitionId", model.OriginalData.TaskDefinition.Oid))
                .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                .Add(Expression.IsNull("Lifetime.Termination"))
                .SetProjection(Projections.Property("SubscriberId"));

            model.NotificationList = NHSession.CreateCriteria<Person>()
                .Add(Subqueries.PropertyIn("Oid", detachedCriteria))
                .AddOrder(new Order("FirstName", true))
                .List<Person>();

            if (model.OriginalData.TaskDefinition.Owner.Oid == CurrentIdentity.ID ||
                model.NotificationList.Where(p => p.Oid == CurrentIdentity.ID).Count() > 0)
            {
                model.HasAccess = true;
            }

            //if(model.HasAccess == false){
            //    throw new SecurityException(string.Format("User: {0} does not have access to taskDefinition:{1}", CurrentIdentity.ID, model.OriginalData.TaskDefinition.Oid));
            //}

            this.BreadCrumbs[0].Url = model.ReturnUrl;

        }

        #endregion


    }
    public class UserTaskMapping
    {
        public string userName;
        public List<string> tasks;
    }
}
