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
    /// Task Status
    /// </summary>
    public partial class TaskType :  LookupBase<long>
    {
        
    }
}
