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
    public partial class TaskDefinition :  DataObject<long>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public virtual Person Owner { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public virtual string Priority { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public virtual Object Object { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public virtual Int64 TaskType{ get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public virtual Int64 Frequency { get; set; }

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public virtual DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Get the list of task associated with the task def
        /// </summary>
        public virtual IList<Task> Tasks { get; set; }
    }
}
