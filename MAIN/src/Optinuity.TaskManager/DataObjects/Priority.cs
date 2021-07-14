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
    /// Priority
    /// </summary>
    public class Priority 
    {
        /// <summary>
        /// Gets the high priority code.
        /// </summary>
        /// <value>
        /// The high.
        /// </value>
        public string High { get { return "H"; } }

        /// <summary>
        /// Gets the medium priority code.
        /// </summary>
        /// <value>
        /// The medium.
        /// </value>
        public string Medium { get { return "M"; } }

        /// <summary>
        /// Gets the low priority code.
        /// </summary>
        /// <value>
        /// The low.
        /// </value>
        public string Low { get { return "L"; } }
    }
}
