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
    /// Resource Object 
    /// </summary>
    public partial class Object :  DataObject<long>
    {


        /// <summary>
        /// Gets or sets the object type identifier.
        /// </summary>
        /// <value>
        /// The object type identifier.
        /// </value>
        public virtual  long ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        public virtual string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the object key.
        /// </summary>
        /// <value>
        /// The object key.
        /// </value>
        public virtual string ObjectKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public virtual string ObjectName { get; set; }

        /// <summary>
        /// Get the object name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ObjectName;
        }
    }
}
