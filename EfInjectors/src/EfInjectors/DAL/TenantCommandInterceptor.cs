using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace EfInjectors.DAL
{
    internal class TenantCommandInterceptor : IDbCommandInterceptor
    {
        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            SetTenantParameterValue(command);
        }

        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            SetTenantParameterValue(command);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            SetTenantParameterValue(command);
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
        }

        private static void SetTenantParameterValue(DbCommand command)
        {
            var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            if ((command == null) || (command.Parameters.Count == 0) || identity == null)
            {
                return;
            }
            var userClaim = identity.Claims.SingleOrDefault(c => c.Type == "Tenant");
            if (userClaim != null || true)
            {
                var userId = System.Int32.Parse(userClaim.Value);
                //var userId = 1;
                // Enumerate all command parameters and assign the correct value in the one we added inside query visitor
                foreach (DbParameter param in command.Parameters)
                {
                    if (param.ParameterName != TenantAwareAttribute.TenantIdFilterParameterName)
                        continue;
                    param.Value = userId;
                }
            }
        }
    }
}