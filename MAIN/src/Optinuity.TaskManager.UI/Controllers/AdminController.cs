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
using System.Security.Principal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace Optinuity.TaskManager.UI.Controllers
{
    
    public class AdminController : UIControllerBase
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("AdminController");
        /// <summary>
        /// Defalt page for Admin controller
        /// </summary>
        /// <param name="ddlStatus">task status</param>
        /// <returns></returns>
        public ActionResult Index(string ddlStatus, string ddlOwnership, string txtSearchDesc, string output = "html")
        {
            LoggerBase.Logger.Info("Inside AdminController Index method");
            LoggerBase.Logger.Info("Current User ID: " + CurrentIdentity.ID);
            
            Int64 currentIdentity = CurrentIdentity.ID;
            //if (string.IsNullOrEmpty(ddlOwnership))
            //    ddlOwnership = currentIdentity.ToString();

            //currentIdentity = 1143; //tim
            //currentIdentity = 296016; //hirdey
            //currentIdentity = 2201; //charlie
            LoggerBase.Logger.Info("OutPut Variable Value: " + output);
            
            if (output == "excel")
            {
                NHibernate.ICriteria criteria = NHSession.CreateCriteria(typeof(TaskDefinitionForReport));

                if (!HttpContext.User.IsInRole("TaskManager_Task Admin"))
                    criteria.Add(Expression.Eq("OwnerId", currentIdentity));
                else
                {

                    if (!string.IsNullOrEmpty(ddlOwnership))
                    {
                        
                        Int64 OwnershipId = Convert.ToInt64(ddlOwnership);
                        LoggerBase.Logger.Info("Ownership ID: " + OwnershipId);
                        if (OwnershipId != 0)
                            criteria.Add(Expression.Eq("OwnerId", OwnershipId));
                    }
                
                }

                if (ddlStatus != "all")//Currently active or all
                    criteria.Add(Expression.Gt("EndDate", DateTime.Now.AddDays(-1)));

                criteria.AddOrder(Order.Asc("TaskCreatedDate"));

                IList<TaskDefinitionForReport> model = criteria.List<TaskDefinitionForReport>();
                if (!string.IsNullOrEmpty(txtSearchDesc))
                    model = model.Where(x => x.TaskDescription != null && x.TaskDescription.ToLower().Contains(txtSearchDesc.ToLower()) || x.TaskName.ToLower().Contains(txtSearchDesc.ToLower())).ToList();
                
                return excelSearchResult(model.ToList());
            }
            else
            {

                NHibernate.ICriteria criteria = NHSession.CreateCriteria(typeof(TaskDefinition));
                criteria.CreateAlias("Object", "Object");
                criteria.CreateAlias("Owner", "Owner");

                if (!HttpContext.User.IsInRole("TaskManager_Task Admin"))
                {
                    criteria.Add(Expression.Eq("Owner.Oid", currentIdentity));
                    ViewBag.IsAdmin = false;
                }

                if (ddlStatus != "all")//Currently active or all
                    criteria.Add(Expression.Gt("EndDate", DateTime.Now.AddDays(-1)));

                criteria.AddOrder(Order.Asc("CreatedDate"));

                IList<TaskDefinition> model = criteria.List<TaskDefinition>();

                if (HttpContext.User.IsInRole("TaskManager_Task Admin"))
                {
                    
                    var OwnerList = model.ToList()
                        .Select(x => x.Owner).Distinct()
                        .OrderBy(x => x.FullName)
                        .Select(s => new SelectListItem { Text = s.FullName, Value = s.Oid.ToString() })
                        .ToList();
                    OwnerList.Insert(0, new SelectListItem { Text = "All", Value = "0" });
                    ViewBag.OwnerList = OwnerList;
                    ViewBag.IsAdmin = true;
                }

                if (!string.IsNullOrEmpty(ddlOwnership))
                {
                    Int64 OwnershipId = Convert.ToInt64(ddlOwnership);
                    if (OwnershipId != 0)
                    {
                        model = model.ToList()
                            .Where(x => x.Owner.Oid == OwnershipId)
                            .ToList();
                    }
                }

                if (!string.IsNullOrEmpty(txtSearchDesc))
                    model = model.Where(x => x.Description != null && x.Description.ToLower().Contains(txtSearchDesc.ToLower()) || x.Name.ToLower().Contains(txtSearchDesc.ToLower())).ToList();
                
                ViewBag.txtSearchDesc = txtSearchDesc;
                return View("TaskDefinitionListing", model);
            }
        }
        

        private ActionResult excelSearchResult(List<TaskDefinitionForReport> model)
        {

            FileContentResult result;
            var formattedData = model.Select(j => new
            {
                Name = j.TaskName,
                Desc = ExcelUtility.StripHTML(j.TaskDescription),
                Owner = j.Owner,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                ReminderDays = j.ReminderDays,
                TaskCreatedDate = j.TaskCreatedDate,
                TaskFrequency = j.TaskFrequency,
                TaskSubscriber = j.TaskSubscriber
                
            }).ToList<object>();

            List<ColumnMapping> mapping = new List<ColumnMapping>(){
                new ColumnMapping{ ColumnName = "Name", ObjectName = "Name", AutoFitColumn = true} ,    
                new ColumnMapping{ ColumnName = "Description", ObjectName = "Desc", AutoFitColumn = true} ,    
                new ColumnMapping{ ColumnName = "Owner", ObjectName = "Owner", AutoFitColumn = true} ,    
                new ColumnMapping{ ColumnName = "Start Date", ObjectName = "StartDate", AutoFitColumn = true} ,
                new ColumnMapping{ ColumnName = "End Date", ObjectName = "EndDate", AutoFitColumn = true} ,
                new ColumnMapping{ ColumnName = "Reminder Days", ObjectName = "ReminderDays", AutoFitColumn = true} ,
                new ColumnMapping{ ColumnName = "Frequency", ObjectName = "TaskFrequency", AutoFitColumn = true} ,
                new ColumnMapping{ ColumnName = "Subscriber", ObjectName = "TaskSubscriber", AutoFitColumn = true} ,
                new ColumnMapping{ ColumnName = "Created Date", ObjectName = "TaskCreatedDate", AutoFitColumn = true} 
            };

            using (MemoryStream stream = ExcelUtility.ExportDynamic(formattedData, mapping, "Tasks"))
            {
                result = File(stream.ToArray(), "application/vnd.ms-excel", "Tasks.xlsx");
            }

            return result;
        }

        #region Admin Screens

        // TO DO: Make sure he is the owner
        /// <summary>
        /// Manages the tasks.
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageTasks(long? TaskId, string message= "" ) {
            TaskDefinationVM model = new TaskDefinationVM();
            LoggerBase.Logger.Info("Inside ManageTasks Method");
            LoggerBase.Logger.Info("TaskId: " + TaskId);

            loadTaskInfo(model, TaskId);
            if (message != null & message.Length > 0)
                model.SuccessfulMessage = message;
            return View(model);
        }

        /// <summary>
        /// This method is called for an action in managed task
        /// </summary>
        /// <param name="model">the model</param>
        /// <param name="btnSave">clicked button value</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ManageTasks(TaskDefinationVM model, string btnSave)
        {
            LoggerBase.Logger.Info("Inside ManageTasks Post Method");

            model = PopulateDropdowns(model);
            LoggerBase.Logger.Info("btnSave value: " + btnSave);
            #region Delete Task
            if (btnSave == "delete")
            {
                LoggerBase.Logger.Info("Task ID: " + model.TaskId);

                var oriTaskDef = NHSession.CreateCriteria<TaskDefinition>()
                                    .Add(Expression.Eq("Oid", model.TaskId))
                                    .List<TaskDefinition>().FirstOrDefault();
                // check if the user is the owner or has admin previlage
                if (oriTaskDef.Owner.Oid != CurrentIdentity.ID)// you are not the owner
                {
                    if (!HttpContext.User.IsInRole("TaskManager_Task Admin")) // And you are not the task admin
                    {
                        throw new SecurityException(string.Format("User: {0} does not have access to taskDefinition:{1}. Original Task Owner {2}", CurrentIdentity.ID, model.TaskId, oriTaskDef.Owner.Oid));
                    }
                }
                // check if all the status are pending - then delete the task and task definations
                if (CheckForPending(oriTaskDef))
                {
                    DeleteTaskDefination(oriTaskDef);
                }
                else
                {
                    // make sure end date is not greater the start date as oracle will throw an error
                    if (oriTaskDef.StartDate > DateTime.Now.AddDays(-1))
                        oriTaskDef.EndDate = oriTaskDef.StartDate;
                    else
                        oriTaskDef.EndDate = DateTime.Now.AddDays(-1);
                    NHSession.SaveOrUpdate(oriTaskDef);
                }
                
                NHSession.Transaction.Commit();
                //AddMessage(new Message { MessageType = MessageType.Information, Title = "Task definition has been deleted successfully" });
                ViewBag.DeleteConfirmation = true;
                return Index("","","");
            }
            #endregion
            else if (btnSave != "save")
                return View("ManageTasks", model);

            //// model is not in a valid state
            if (!ModelState.IsValid)
            {
                model.HasAccess = true;
                return View("ManageTasks", model);
            }
            bool isNew = false;
            Optinuity.Framework.DataObjects.Lifetime lifetime = new Framework.DataObjects.Lifetime();
            lifetime.Inception = DateTime.Now;
            
            if (model.TaskId == 0)
            {
                LoggerBase.Logger.Info("New Task Defination");
                #region New Task Defination
                isNew = true;
                //Step 1: Check if new or existing
                Person person = new Person();
                person.Oid = model.OwnerId;
                //model.OwnerId = CurrentIdentity.ID;
                var currentUser = NHSession.CreateCriteria<Person>()
                    .Add(Expression.Eq("Oid", model.OwnerId))
                   //.Add(Expression.Eq("Oid", CurrentIdentity.ID))
                   .List<Person>().FirstOrDefault();

                var objectkey = NHSession.CreateCriteria<DataObjects.Object>()
                   .Add(Expression.Eq("ObjectKey", currentUser.DivisionCode))
                   .List<DataObjects.Object>().FirstOrDefault();

                
                //long taskDefId = AddTaskDefination(model.Name, model.Description, model.StartDate, model.EndDate.Value, p, obj, model.FrequencyListValue);
                TaskDefinition taskDef = new TaskDefinition();
                taskDef.Name = model.Name;
                taskDef.Description = model.Description;
                taskDef.StartDate = model.StartDate.Value;
                if (model.EndDate == null)
                    taskDef.EndDate = model.StartDate;
                else
                    taskDef.EndDate = model.EndDate;
                taskDef.Owner = person;
                taskDef.Object = objectkey;
                // Hardcodes to 7 based on conversation with Hirdey... this might create more confusion
                taskDef.TaskType = 7;
                taskDef.Frequency = Convert.ToInt64(model.FrequencyListValue);
                taskDef.Priority = "H"; // The priority is always High for Tasks

                NHSession.SaveOrUpdate(taskDef);
                model.TaskId = taskDef.Oid;
                model.TaskIdString = taskDef.Oid.ToString();
                List<string> userLists = model.PersonListValue.Split(',').ToList<string>();
                List<string> userListForEmail = new List<string>();
                
                foreach (var userList in userLists)
                {
                    AddSubscription(Convert.ToInt64(userList), model.WaitingPeriod, taskDef.Oid, lifetime);
                    userListForEmail.Add(userList);
                }
                NHSession.Transaction.Commit();
                
                model.OwnerValue = CurrentIdentity.FirstName + " " +CurrentIdentity.LastName;
                model.NotificationListValue = Helpers.UIHelper.GetUserAttribute(NHSession, userListForEmail, null, Helpers.UIHelper.UserAttribute.FullName); 
                Helpers.UIHelper.SendEmail(NHSession, userListForEmail, CurrentIdentity.ID, UIHelper.EmailType.NewTask, model.TaskId.ToString(), model.Name, CurrentIdentity.Email);
                
                #endregion
            }
            else
            {
                LoggerBase.Logger.Info("Update Task Defination");
                LoggerBase.Logger.Info("Task ID: " + model.TaskId);

                #region Update Task Defination
                bool changed = false;
                var oriTaskDef = NHSession.CreateCriteria<TaskDefinition>()
                    .Add(Expression.Eq("Oid", model.TaskId))
                    .List<TaskDefinition>().FirstOrDefault();

                // check if the user is the owner or has admin previlage
                //if (oriTaskDef.Owner.Oid != CurrentIdentity.ID || !Roles.IsUserInRole("TaskAdmin"))
                //throw new SecurityException(string.Format("User: {0} does not have access to taskDefinition:{1}", CurrentIdentity.ID, model.TaskId));

                if (oriTaskDef.Owner.Oid != CurrentIdentity.ID)// you are not the owner
                {
                    if (!HttpContext.User.IsInRole("TaskManager_Task Admin")) // And you are not the task admin
                    {
                        throw new SecurityException(string.Format("User: {0} does not have access to taskDefinition:{1}. Original Task Owner {2}", CurrentIdentity.ID, model.TaskId, oriTaskDef.Owner.Oid));
                    }
                }

                if (model.Name != oriTaskDef.Name)
                {
                    changed = true;
                    oriTaskDef.Name = model.Name;
                }

                if (model.Description != oriTaskDef.Description)
                {
                    changed = true;
                    oriTaskDef.Description = model.Description;
                }
                
                if (model.StartDate != oriTaskDef.StartDate)
                {
                    changed = true;
                    oriTaskDef.StartDate = model.StartDate.Value;
                }
                
                if (model.EndDate != oriTaskDef.EndDate)
                {
                    changed = true;
                    oriTaskDef.EndDate = model.EndDate;
                }
                
                if (model.FrequencyListValue != oriTaskDef.Frequency)
                {
                    changed = true;
                    oriTaskDef.Frequency= model.FrequencyListValue;
                }

                if (model.OwnerId!= oriTaskDef.Owner.Oid)
                {
                    changed = true;
                    Person person = new Person();
                    person.Oid = model.OwnerId;
                    oriTaskDef.Owner = person;
                    List<string> newOwner = new List<string>();
                    newOwner.Add(model.OwnerId.ToString());
                    Helpers.UIHelper.SendEmail(NHSession, newOwner, CurrentIdentity.ID, UIHelper.EmailType.OwnerChanged, model.TaskId.ToString(), model.Name, CurrentIdentity.Email);
                }
                model.OwnerValue = Helpers.UIHelper.GetUserAttribute(NHSession, null, model.OwnerId, Helpers.UIHelper.UserAttribute.FullName); 

                var subscription = NHSession.CreateCriteria<Subscription>()
                    .Add(Expression.Eq("TaskDefinitionId", model.TaskId))
                    .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                    .Add(Expression.IsNull("Lifetime.Termination"))
                    .List<Subscription>().FirstOrDefault();

                LoggerBase.Logger.Info("Before update the tck_task_subscriptions");

                // Need to update the tck_task_subscriptions
                if (subscription != null)
                {
                    if (model.WaitingPeriod != subscription.WaitPeriod)
                    {
                        // in this case you need to update all the subscriptions
                        var subscriptions = NHSession.CreateCriteria<Subscription>()
                            .Add(Expression.Eq("TaskDefinitionId", model.TaskId))
                            .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                            .Add(Expression.IsNull("Lifetime.Termination"))
                            .List<Subscription>();
                        foreach (var sub in subscriptions)
                        {
                            if (model.WaitingPeriod > 0)
                                sub.WaitPeriod = model.WaitingPeriod * -1; //waiting period is always saved as negative
                            else
                                sub.WaitPeriod = model.WaitingPeriod; 
                            //sub.WaitPeriod = model.WaitingPeriod;
                            sub.Audit.UpdatedDate = DateTime.Now;
                            NHSession.SaveOrUpdate(sub);
                        }
                    }
                }
                if (changed)
                    NHSession.SaveOrUpdate(oriTaskDef);
                LoggerBase.Logger.Info("After update the tck_task_subscriptions");
                

                DetachedCriteria detachedCriteria = DetachedCriteria.For<Subscription>()
                        .Add(Expression.Eq("TaskDefinitionId", model.TaskId))
                        .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                        .Add(Expression.IsNull("Lifetime.Termination"))
                        .SetProjection(Projections.Property("SubscriberId"));

                var notificationList = NHSession.CreateCriteria<Person>()
                    .Add(Subqueries.PropertyIn("Oid", detachedCriteria))
                    .AddOrder(new Order("FirstName", true))
                    .List<Person>();

                List<string> existingUserList = new List<string>();
                foreach (var x in notificationList)
                    existingUserList.Add(x.Oid.ToString());

                List<string> newuserList = model.PersonListValue.Split(',').ToList<string>();
                model.NotificationListValue = Helpers.UIHelper.GetUserAttribute(NHSession, newuserList, null, Helpers.UIHelper.UserAttribute.FullName); 
                List<string> toBeDeleted = existingUserList.Except(newuserList).ToList();
                List<string> toBeAdded = newuserList.Except(existingUserList).ToList();

                foreach (var x in toBeDeleted)
                {
                    var subscriptions = NHSession.CreateCriteria<Subscription>()
                        .Add(Expression.Eq("TaskDefinitionId", model.TaskId))
                        .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                        .Add(Expression.IsNull("Lifetime.Termination"))
                        .Add(Expression.Eq("SubscriberId",Convert.ToInt64(x)))
                        .List<Subscription>().FirstOrDefault();
                    subscriptions.Lifetime.Termination = DateTime.Now;
                    NHSession.SaveOrUpdate(subscriptions);
                }

                foreach (var x in toBeAdded)
                    AddSubscription(Convert.ToInt64(x), model.WaitingPeriod, model.TaskId, lifetime);
                NHSession.Transaction.Commit();

                if (toBeAdded.Count > 0)
                    Helpers.UIHelper.SendEmail(NHSession, toBeAdded, CurrentIdentity.ID, UIHelper.EmailType.UserAdded, model.TaskId.ToString(), model.Name, CurrentIdentity.Email);
                if (toBeDeleted.Count > 0)
                    Helpers.UIHelper.SendEmail(NHSession, toBeDeleted, CurrentIdentity.ID, UIHelper.EmailType.UserRemoved, model.TaskId.ToString(), model.Name, CurrentIdentity.Email);
                #endregion
            }

            if(isNew)
                model.SuccessfulMessage = "Task definition has been created successfully";
            else
                model.SuccessfulMessage = "Task definition has been updated successfully";
            
            model.TaskIdString = model.TaskId.ToString();
            
            if (model.PersonListValue != null)
            {
                model.PersonListSelected = model.PersonListValue;
                List<string> userLists = model.PersonListValue.Split(',').ToList<string>();
                foreach (var x in userLists)
                {
                    var selectedItem = model.PersonList.Where(q => q.Value == x)
                    .FirstOrDefault();
                    if (selectedItem != null) selectedItem.Selected = true;
                }
            }
            return View("ManageTasks", model);
        }

        

        /// <summary>
        /// Adding Task Definations
        /// </summary>
        /// <param name="name">def name</param>
        /// <param name="desc">def desc</param>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="owner">owner</param>
        /// <param name="obj">person object of the owner</param>
        /// <param name="frequency">frequency</param>
        /// <returns></returns>
        long AddTaskDefination(string name, string desc, DateTime startDate, DateTime endDate, Person owner, DataObjects.Object obj, long frequency)
        {
            TaskDefinition taskDef = new TaskDefinition();
            taskDef.Name = name;
            taskDef.Description = desc;
            taskDef.StartDate = startDate;
            taskDef.EndDate = endDate;
            taskDef.Owner = owner;
            taskDef.Object = obj;
            // Hardcodes to 7 based on conversation with Hirdey... this might create more confusion
            taskDef.TaskType = 7;
            taskDef.Frequency = frequency;
            taskDef.Priority = "H";
            NHSession.SaveOrUpdate(taskDef);
            return taskDef.Oid;

        }

        /// <summary>
        /// Adding Subscription to database
        /// </summary>
        /// <param name="userId">user id to be addes</param>
        /// <param name="waitingPeriod">reminder days</param>
        /// <param name="taskDefId">parent task def id</param>
        /// <param name="lifeTime">end date</param>
        /// <returns></returns>
        long AddSubscription(long userId, long waitingPeriod, long taskDefId, Framework.DataObjects.Lifetime lifeTime)
        {
            Subscription sub = new Subscription();
            sub.SubscriberId = userId;
            sub.SubscriberSourceId = SubscriberSourceExtension.MBIA;
            sub.SubscriptionType = "N";
            if (waitingPeriod > 0)
                sub.WaitPeriod = waitingPeriod * -1; //waiting period is always saved as negative
            else
                sub.WaitPeriod = waitingPeriod;
            sub.RepeatUntilCompleted = "Y";
            sub.RepeatFrequency = 1;
            sub.TaskDefinitionId = taskDefId;
            sub.Lifetime = lifeTime;
            NHSession.SaveOrUpdate(sub);
            return sub.Oid;
        }

        /// <summary>
        /// Populating Dropdowns for the screens
        /// </summary>
        /// <param name="model">the model</param>
        /// <returns></returns>
        TaskDefinationVM PopulateDropdowns(TaskDefinationVM model)
        {
            // Add status list
            model.TypeList = NHSession.CreateCriteria<TaskType>()
                .List<TaskType>().Select(s => new SelectListItem { Text = s.ShortDescription, Value = s.Oid.ToString() })
                .ToList();
            model.TypeList.Insert(0, new SelectListItem { Text = "Any", Value = "0" });

            // Add status list
            var freqIn = new string[] { "W","M", "Q", "O", "Y"};
            model.FrequencyList = NHSession.CreateCriteria<TaskFrequency>()
                .Add(Expression.IsNull("Lifetime.Termination"))
                .Add(Expression.In("AbbreviatedDescription", freqIn))
                .List<TaskFrequency>().Select(s => new SelectListItem { Text = s.ShortDescription, Value = s.Oid.ToString() })
                .ToList();
            model.FrequencyList.Insert(0, new SelectListItem { Text = "Select", Value = "0" });
            
            List<SelectListItem> months = new List<SelectListItem>();
            model.MonthList = months;
            model.MonthList.Insert(0, new SelectListItem { Text = "Jan", Value = "1" });
            model.MonthList.Insert(1, new SelectListItem { Text = "Feb", Value = "2" });
            model.MonthList.Insert(2, new SelectListItem { Text = "Mar", Value = "3" });
            model.MonthList.Insert(3, new SelectListItem { Text = "Apr", Value = "4" });
            model.MonthList.Insert(4, new SelectListItem { Text = "May", Value = "5" });
            model.MonthList.Insert(5, new SelectListItem { Text = "Jun", Value = "6" });
            model.MonthList.Insert(6, new SelectListItem { Text = "Jul", Value = "7" });
            model.MonthList.Insert(7, new SelectListItem { Text = "Aug", Value = "8" });
            model.MonthList.Insert(8, new SelectListItem { Text = "Sep", Value = "9" });
            model.MonthList.Insert(9, new SelectListItem { Text = "Oct", Value = "10" });
            model.MonthList.Insert(10, new SelectListItem { Text = "Nov", Value = "11" });
            model.MonthList.Insert(11, new SelectListItem { Text = "Dec", Value = "12" });
            string[] personType = {"Employee", "Temp", "Contractor","Consultant","Intern"};
            //string[] personType = { "Employee" };// Only employees can be assigned to a task

            model.PersonList = NHSession.CreateCriteria<Person>()
                .Add(Expression.IsNull("Lifetime.Termination"))
                .Add(Expression.In("PersonType", personType))
                .AddOrder(new Order("FirstName", true))
                .List<Person>().Select(s => new SelectListItem { Text = s.FirstName + " " + s.LastName, Value = s.Oid.ToString() })
                .ToList();

            model.OwnerList = model.PersonList;

            return model;
        }
        
        /// <summary>
        /// Loads the task information.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <exception cref="System.Security.SecurityException"></exception>
        public void loadTaskInfo(TaskDefinationVM model, long? taskId)
        {
            model = PopulateDropdowns(model);
            model.TaskIdString = model.TaskId.ToString();
            model.OwnerId = CurrentIdentity.ID;
            if (taskId != null)// 0 for new
            {
                model.TaskId = taskId.Value;
                var originalTaskDefinition = NHSession.CreateCriteria<TaskDefinition>()
                    .CreateAlias("Owner", "Owner")
                    .CreateAlias("Object", "TaskDefinition_Object")
                    .Add(Expression.Eq("Oid", taskId))
                    .List<TaskDefinition>().FirstOrDefault();

                model.Name = originalTaskDefinition.Name;
                model.Description = originalTaskDefinition.Description;
                model.StartDate = originalTaskDefinition.StartDate;
                model.EndDate = originalTaskDefinition.EndDate;
                model.TaskId = originalTaskDefinition.Oid;
                model.FrequencyListValue = originalTaskDefinition.Frequency;
                model.OwnerId = originalTaskDefinition.Owner.Oid;
                model.OwnerValue = originalTaskDefinition.Owner.FullName;

                // check if the current owner is not terminated...
                var xa= model.PersonList.Where(x=> x.Value == originalTaskDefinition.Owner.Oid.ToString()).FirstOrDefault();
                if(xa == null)
                {
                    SelectListItem item = new SelectListItem { Text = originalTaskDefinition.Owner.FullName, Value = originalTaskDefinition.Owner.Oid.ToString(), Selected = true };
                    model.PersonList.Add(item);
                }
                

                if (originalTaskDefinition.Owner.Oid == CurrentIdentity.ID)
                    model.HasAccess = true;
                else
                    model.HasAccess = false;
                if (HttpContext.User.IsInRole("TaskManager_Task Admin"))
                    model.HasAccess = true;

                var subscription = NHSession.CreateCriteria<Subscription>()
                    .Add(Expression.Eq("TaskDefinitionId", originalTaskDefinition.Oid))
                    .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                    .Add(Expression.IsNull("Lifetime.Termination"))
                    .List<Subscription>().FirstOrDefault();

                if (subscription != null)
                {
                    int waitingPeriod = Convert.ToInt32(subscription.WaitPeriod.Value);
                    if (waitingPeriod < 0)
                        model.WaitingPeriod = waitingPeriod * -1;
                    else
                        model.WaitingPeriod = waitingPeriod;
                    //model.FrequencyListValue = subscription.RepeatFrequency.Value;
                }

                DetachedCriteria detachedCriteria = DetachedCriteria.For<Subscription>()
                    .Add(Expression.Eq("TaskDefinitionId", originalTaskDefinition.Oid))
                    .Add(Expression.Eq("SubscriberSourceId", SubscriberSourceExtension.MBIA))
                    .Add(Expression.IsNull("Lifetime.Termination"))
                    .SetProjection(Projections.Property("SubscriberId"));


                var notificationList = NHSession.CreateCriteria<Person>()
                    .Add(Subqueries.PropertyIn("Oid", detachedCriteria))
                    .AddOrder(new Order("FirstName", true))
                    .List<Person>();

                string selectedPersonId = "";
                string selectedPersonName = "";
                foreach (var x in notificationList)
                {
                    var selectedItem = model.PersonList.Where(q => q.Value == x.Oid.ToString())
                    .FirstOrDefault();
                    selectedPersonId = selectedPersonId + x.Oid.ToString() + ",";
                    selectedPersonName = selectedPersonName + x.FullName + ",";
                    if (selectedItem != null) 
                        selectedItem.Selected = true;
                }
                if (selectedPersonId.Length > 1)
                    model.PersonListValue = selectedPersonId.Substring(0, selectedPersonId.Length - 1);
                if (selectedPersonName.Length > 1)
                    model.NotificationListValue = selectedPersonName.Substring(0, selectedPersonName.Length - 1);
            }
            else
            {
                model.HasAccess = true;
            }
        }

        bool CheckForPending(TaskDefinition taskDefination)
        {
            foreach (var x in taskDefination.Tasks)
            { 
                if(x.TaskStatus.ToString() != "Pending")
                    return false;
            }
            return true;
        }

        void DeleteTaskDefination(TaskDefinition taskDefination)
        {
            var subscriptions = NHSession.CreateCriteria<Subscription>()
                                   .Add(Expression.Eq("TaskDefinitionId", taskDefination.Oid))
                                   .List<Subscription>();

            foreach (var subscription in subscriptions)
            {
                NHSession.Delete(subscription);
            }

            foreach (var task in taskDefination.Tasks)
            {
                NHSession.Delete(task);
            }

            
            NHSession.Delete(taskDefination);
            //NHSession.Transaction.Commit();
        }

        #endregion
    }
}
