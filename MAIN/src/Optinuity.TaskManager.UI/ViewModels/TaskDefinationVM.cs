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
    /// Task Defination View Model
    /// </summary>
    public class TaskDefinationVM : IValidatableObject
    {
        /// <summary>
        /// Task name
        /// </summary>
        [Required(ErrorMessage = "Title cannot be blank")]
        [StringLength(500)]
        [Display(Name = "Title")]
        [AllowHtml]
        public virtual string Name { get; set; }

        /// <summary>
        /// Task description
        /// </summary>
        [Required(ErrorMessage = "Description cannot be blank")]
        [StringLength(4000)]
        [Display(Name = "Description")]
        [AllowHtml]
        public virtual string Description { get; set; }

        /// <summary>
        /// Inital due date
        /// </summary>
        [Required(ErrorMessage = "Initial due date cannot be blank")]
        [DataType(DataType.Date)]
        [Display(Name = "Inital due date")]
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Final due date
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Final due date")]
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Frequency for task
        /// </summary>
        [Range(1, 30, ErrorMessage = "Please select frequency")]
        public long FrequencyListValue { get; set; }
        public List<SelectListItem> FrequencyList { get; set; }

        /// <summary>
        /// Reminder days
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Please enter reminder days")]
        public int WaitingPeriod { get; set; }

        /// <summary>
        /// PersonListSelected  is only used for validations
        /// </summary>
        [Required(ErrorMessage = "Please select users for notification")]
        public string PersonListSelected { get; set; }

        /// <summary>
        /// Person List for drop downs
        /// </summary>
        public List<SelectListItem> PersonList { get; set; }

        /// <summary>
        /// Person List Value that is selected
        /// </summary>
        public string PersonListValue { get; set; }

        /// <summary>
        /// check if user has access
        /// </summary>
        public bool HasAccess { get; set; }

        /// <summary>
        /// Type for the dropdowns
        /// </summary>
        public List<SelectListItem> TypeList { get; set; }

        /// <summary>
        /// Type of the month dropdown
        /// </summary>
        public List<SelectListItem> MonthList { get; set; }

        /// <summary>
        /// Dropdown for the person
        /// </summary>
        public List<SelectListItem> OwnerList { get; set; }

        /// <summary>
        /// Owner Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// task id
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// task id as string
        /// </summary>
        public string TaskIdString { get; set; }

        /// <summary>
        /// Successful Message to be displayed
        /// </summary>
        public string SuccessfulMessage { get; set; }

        /// <summary>
        /// Error message to be displayed
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Displays the name for the notification list
        /// </summary>
        public string NotificationListValue { get; set; }
        /// <summary>
        /// Displays the name for the owner
        /// </summary>
        public string OwnerValue { get; set; }

        /// <summary>
        /// UI validation object
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FrequencyListValue != 1)
            {
                if (EndDate < StartDate)
                {
                    yield return new ValidationResult("Final due date must be greater than initial due date.");
                }
            }
            if (FrequencyListValue != 1 && EndDate == null)
            {
                yield return new ValidationResult("Please enter final due date.");
            }

            //if (WaitingPeriod < 1)
            //{
            //    yield return new ValidationResult("Waiting period has to be greater then 0");
            //}

        }

    }
}