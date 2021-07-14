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
    /// Person
    /// </summary>
    public partial class Person :  DataObjectWithLifetime<long>
    {
        /// <summary>
        /// First Name
        /// </summary>
        public virtual string FirstName { get; set; }
        
        /// <summary>
        /// Last Name
        /// </summary>
        public virtual string LastName { get; set; }
        
        /// <summary>
        /// Nick Name
        /// </summary>
        public virtual string NickName { get; set; }

        /// <summary>
        /// Department Name
        /// </summary>
        public virtual string DepartmentName { get; set; }

        /// <summary>
        /// Business Unit Name
        /// </summary>
        public virtual string BusinessUnitName { get; set; }

        /// <summary>
        /// Business Unit Code
        /// </summary>
        public virtual string BusinessUnitCode { get; set; }
        /// <summary>
        /// Division Code
        /// </summary>
        public virtual string DivisionCode { get; set; }
        
        /// <summary>
        /// Email Address
        /// </summary>
        public virtual string Email { get; set; }
        
        /// <summary>
        /// Login Name (i.e., network id 5+1)
        /// </summary>
        public virtual string LoginName { get; set; }

        /// <summary>
        /// Person Type
        /// </summary>
        public virtual string PersonType { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public virtual string Company { get; set; }
        
        /// <summary>
        /// Company
        /// </summary>
        public virtual int ManagerId { get; set; }
        

        /// <summary>
        /// Termination Date
        /// </summary>
        public virtual DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Manager
        /// </summary>
        public virtual Person Manager { get; set; }

        /// <summary>
        /// GL Code
        /// </summary>
        public virtual string GlCode { get; set; }

        /// <summary>
        /// Gets Full Name.
        /// </summary>
        public virtual string FullName { get { return string.Format("{0} {1}", (NickName != null) ? NickName : FirstName, LastName); } }

        /// <summary>
        /// Gets Full Name Reverse.
        /// </summary>
        public virtual string FullNameReverse
        {
            get { return string.Format("{1}, {0}", (NickName != null) ? NickName : FirstName, LastName); }
        }


        /// <summary>
        /// Return full name of the person
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullNameReverse;
        }

        /// <summary>
        /// Friendly Name
        /// </summary>
        public virtual string FriendlyName
        {
            get {return  ((NickName != null) ? NickName : FirstName); }
        }
    }
}
