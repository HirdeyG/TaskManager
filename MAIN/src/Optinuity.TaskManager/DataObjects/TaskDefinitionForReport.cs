using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optinuity.Framework.DataObjects;
using NHibernate;
using Optinuity.Framework.NHibernate;
using NHibernate.Criterion;

namespace Optinuity.TaskManager.DataObjects
{
    /// <summary>
    /// Task Definition
    /// </summary>
    public partial class TaskDefinitionForReport :  DataObject<long>
    {
        /// <summary>
        /// Task Name
        /// </summary>
        public virtual string TaskName { get; set; }
        /// <summary>
        /// Task Description
        /// </summary>
        public virtual string TaskDescription { get; set; }
        /// <summary>
        /// Task Owner
        /// </summary>
        public virtual string Owner { get; set; }
        /// <summary>
        /// Start Date
        /// </summary>
        public virtual DateTime StartDate { get; set; }
        /// <summary>
        /// End Date
        /// </summary>
        public virtual DateTime EndDate { get; set; }
        /// <summary>
        /// Reminder Days
        /// </summary>
        public virtual string ReminderDays { get; set; }
        /// <summary>
        /// Task Created Date
        /// </summary>
        public virtual DateTime TaskCreatedDate { get; set; }
        /// <summary>
        /// Frequency
        /// </summary>
        public virtual string TaskFrequency { get; set; }
        /// <summary>
        /// Subscriber name separated with comma
        /// </summary>
        public virtual string TaskSubscriber { get; set; }
        /// <summary>
        /// Task Owner Id 
        /// </summary>
        public virtual Int64 OwnerId { get; set; }
        
    }
}
