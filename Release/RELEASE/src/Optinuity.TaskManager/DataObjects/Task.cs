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
    /// Task
    /// </summary>
    public partial class Task :  DataObject<long>
    {

        /// <summary>
        /// Gets or sets the task definition.
        /// </summary>
        /// <value>
        /// The task definition.
        /// </value>
        public virtual TaskDefinition TaskDefinition { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        public virtual DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public virtual string Comments { get; set; }

        /// <summary>
        /// Gets or sets the task status.
        /// </summary>
        /// <value>
        /// The task status.
        /// </value>
        public virtual TaskStatus TaskStatus { get; set; }

        /// <summary>
        /// Gets or sets the last updated by.
        /// </summary>
        /// <value>
        /// The last updated by.
        /// </value>
        public virtual Person LastUpdatedBy { get; set; }
    }
}
