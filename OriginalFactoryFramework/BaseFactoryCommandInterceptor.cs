using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSD.FactoryFramework
{
    public abstract class BaseFactoryCommandInterceptor : IDbCommandInterceptor
    {
        public virtual void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            SetTenantParameterValue(command, interceptionContext);
        }

        public virtual void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) { }

        public virtual void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            SetTenantParameterValue(command, interceptionContext);
        }

        public virtual void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) { }

        public virtual void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            SetTenantParameterValue(command, interceptionContext);
        }

        public virtual void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) { }

        protected virtual void SetTenantParameterValue(DbCommand command, DbCommandInterceptionContext context)
        {
            System.Security.Principal.IPrincipal identity = null;
            if (System.Web.HttpContext.Current != null)
            {
                identity = System.Web.HttpContext.Current.User;
            }
            else
            {

            }

            //identity = Thread.CurrentPrincipal;

            if ((command == null) || (command.Parameters.Count == 0) || identity == null)
            {
                if (identity == null)
                {
                    return;
                }
                return;
            }

            ////var userClaim = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            ////if (userClaim != null)
            ////{
            ////    var userId = userClaim.Value;
            ////    // Enumerate all command parameters and assign the correct value in the one we added inside query visitor
            ////    foreach (DbParameter param in command.Parameters)
            ////    {
            ////        if (param.ParameterName != TenantAwareAttribute.TenantIdFilterParameterName)
            ////            return;
            ////        param.Value = userId;
            ////    }
            ////}

            // Enumerate all command parameters and assign the correct value in the one we added inside query visitor

            DbParameter parameter = null;

            foreach (DbParameter param in command.Parameters)
            {
                if (param.ParameterName == FactoryConstants.FactoryIdFilterParameterName)
                {
                    param.Value = 1;
                    parameter = param;
                }
            }

            //if (parameter != null)
            //{
            //    command.Parameters.Remove(parameter);
            //}

            foreach (DbParameter param in command.Parameters)
            {
                if (param.ParameterName == FactoryConstants.FactoryOverrideFilterParameterName)
                {
                    param.Value = 0;
                    parameter = param;
                }
            }

            //if (parameter != null)
            //{
            //    command.Parameters.Remove(parameter);
            //}
        }
    }
}