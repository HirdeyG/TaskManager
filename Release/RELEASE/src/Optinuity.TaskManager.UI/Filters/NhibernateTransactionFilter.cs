using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Optinuity.TaskManager.DataObjects;
using Optinuity.TaskManager.BusinessLogic;
using Optinuity.TaskManager.UI.Helpers;
using Optinuity.Framework.NHibernate;
using System.Data;
using Optinuity.Framework.Security;

namespace Optinuity.TaskManager.UI.Filters
{
    /// <summary>
    /// Nhibernate transaction filter
    /// </summary>
    public class NHibernateTransactionFilter: IActionFilter
    {
        private bool _readOnly = true;

        

        
        /// <summary>
        /// Sets session and transaction before action is executed.
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            NHibernateSessionManager.Instance.BeginTransaction();
            //_transactionStarted = true;
            ISession session = NHibernateSessionManager.Instance.GetSession();
            filterContext.Controller.ViewBag.NHSession = session;
            SetUserContext(session.Connection, Application.CurrentIdentity.ID);
            
        }


        /// <summary>
        /// Closes the session and commits the transaction if 
        /// user has specified to do so.
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            bool allowUpdate = filterContext.ActionDescriptor.IsDefined(typeof(AllowNHibernateUpdate), true);

            _readOnly = !allowUpdate;

            try
            {
                object readOnlyKey = HttpContext.Current.Items[NHibernateSessionManager.IsSessionReadOnlyKey];
                if (readOnlyKey != null && readOnlyKey.ToString().ToLower() == "true")
                {
                    _readOnly = true;
                }

                if (_readOnly)
                {
                    NHibernateSessionManager.Instance.RollbackTransaction();
                }
                else
                {
                    NHibernateSessionManager.Instance.CommitTransaction();
                }


            }
            finally
            {
                NHibernateSessionManager.Instance.CloseSession();
            }
        }

        /// <summary>
        /// Sets the user context when a connection is openned
        /// </summary>
        /// <param name="connection"></param>
        public static void SetUserContext(IDbConnection connection, long userId)
        {
            if (connection == null)
            {
                //log.Error("DB User Context not set on null connection.");
            }
            else
            {
                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "begin MBIA_Pk_Audit_Utils.Pr_SetUserSession( :pnPersonId ); end;";
                //cmd.Transaction = (IDbTransaction)NHibernateSessionManager.Instance.GetSession().Transaction;
                IDbDataParameter param = cmd.CreateParameter();
                param.ParameterName = "pnPersonId";
                param.Value = userId;
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Attribute to determine if action method should auto commit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AllowNHibernateUpdate : Attribute { }
}