using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Optinuity.TaskManager.DataObjects;
using System.Web.Mvc;
using Optinuity.Framework.UI;

namespace Optinuity.TaskManager.UI.ViewModels
{

    /// <summary>
    /// Task Listing View Model
    /// </summary>
    public class TaskListingVM : ListingViewModel<Task>
    {
        /// <summary>
        /// Gets or sets the priority list.
        /// </summary>
        /// <value>
        /// The priority list.
        /// </value>
        public List<SelectListItem> PriorityList { get; set; }

        /// <summary>
        /// Gets or sets the due by list.
        /// </summary>
        /// <value>
        /// The due by list.
        /// </value>
        public List<SelectListItem> DueByList { get; set; }

        /// <summary>
        /// Gets or sets the status list.
        /// </summary>
        /// <value>
        /// The status list.
        /// </value>
        public List<SelectListItem> StatusList { get; set; }

        /// <summary>
        /// Gets or sets the task definition list.
        /// </summary>
        /// <value>
        /// The task definition list.
        /// </value>
        public List<SelectListItem> TaskDefinitionList { get; set; }
        /// <summary>
        /// Gets or sets the task definition list for all the task.
        /// </summary>
        //public List<SelectListItem> TaskDefinitionListAll { get; set; }
        public List<SelectListItem> TaskDefinitionListSubOrdinates { get; set; }

        /// <summary>
        /// Gets or sets the selected due date.
        /// </summary>
        /// <value>
        /// The selected due date.
        /// </value>
        public DateTime SelectedDueDate { get; set; }


        /// <summary>
        /// Gets or sets the selected status identifier.
        /// </summary>
        /// <value>
        /// The selected status identifier.
        /// </value>
        public long SelectedStatusId { get; set; }

        /// <summary>
        /// Gets or sets the current URL.
        /// </summary>
        /// <value>
        /// The current URL.
        /// </value>
        public string CurrentUrl { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }
       
        /// <summary>
        ///  Check if the task is expired
        /// </summary>
        public bool HasExpiredItem { get; set; }
        
        /// <summary>
        ///  Check if you need to show all the task
        /// </summary>
        public long EmployeeIdForTask { get; set; }
    }
}