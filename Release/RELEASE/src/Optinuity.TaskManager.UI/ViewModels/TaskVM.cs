using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Optinuity.TaskManager.DataObjects;
using System.Web.Mvc;
using Optinuity.Framework.UI;
using System.ComponentModel.DataAnnotations;

namespace Optinuity.TaskManager.UI.ViewModels
{

    /// <summary>
    /// Task View Model
    /// </summary>
    public class TaskVM 
    {
        /// <summary>
        /// Gets or sets the original data.
        /// </summary>
        /// <value>
        /// The original data.
        /// </value>
        public Task OriginalData { get; set; }

        public TaskDefinition TaskDef { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        [Display(Name = "DueDate")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        [Display(Name="Comments")]
        [StringLength(4000)]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the notification list.
        /// </summary>
        /// <value>
        /// The notification list.
        /// </value>
        public IList<Person> NotificationList { get; set; }


        /// <summary>
        /// Gets or sets the task status identifier.
        /// </summary>
        /// <value>
        /// The task status identifier.
        /// </value>
        public long TaskStatusId { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [has access].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [has access]; otherwise, <c>false</c>.
        /// </value>
        public bool HasAccess { get; set; }


        /// <summary>
        /// Gets or sets the return URL.
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        public string ReturnUrl { get; set; }
    }
}