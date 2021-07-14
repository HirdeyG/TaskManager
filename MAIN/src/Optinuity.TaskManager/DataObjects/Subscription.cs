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
    /// Subscription
    /// </summary>
    public partial class Subscription :  DataObjectWithLifetime<long>
    {

        /// <summary>
        /// Gets or sets the task definition identifier.
        /// </summary>
        /// <value>
        /// The task definition identifier.
        /// </value>
        public virtual long TaskDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the subscriber identifier.
        /// </summary>
        /// <value>
        /// The subscriber identifier.
        /// </value>
        public virtual long SubscriberId { get; set; }

        /// <summary>
        /// Gets or sets the subscriber source identifier.
        /// </summary>
        /// <value>
        /// The subscriber source identifier.
        /// </value>
        public virtual long SubscriberSourceId { get; set; }

        /// <summary>
        /// Gets or sets the type of the subscription.
        /// </summary>
        /// <value>
        /// The type of the subscription.
        /// </value>
        public virtual string SubscriptionType { get; set; }

        /// <summary>
        /// Gets or sets the wait period.
        /// </summary>
        /// <value>
        /// The wait period.
        /// </value>
        public virtual long? WaitPeriod { get; set; }

        /// <summary>
        /// Gets or sets the repeat until completed.
        /// </summary>
        /// <value>
        /// The repeat until completed.
        /// </value>
        public virtual string RepeatUntilCompleted { get; set; }

        /// <summary>
        /// Gets or sets the repeat frequency.
        /// </summary>
        /// <value>
        /// The repeat frequency.
        /// </value>
        public virtual long? RepeatFrequency { get; set; }

        
    }
}
