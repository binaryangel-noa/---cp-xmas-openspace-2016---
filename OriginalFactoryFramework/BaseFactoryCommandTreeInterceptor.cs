using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSD.FactoryFramework
{
    public abstract class BaseFactoryCommandTreeInterceptor : IDbCommandTreeInterceptor
    {
        public virtual void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                // Check that there is an authenticated user in this context
                var identity = Thread.CurrentPrincipal.Identity;
                if (identity == null)
                {
                    return;
                }
                //var userIdclaim = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                //if (userIdclaim == null)
                //{
                //    return;
                //}
                // In case of query command change the query by adding a filtering based on tenantId
                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    var newQuery = queryCommand.Query.Accept(new FactoryQueryVisitor());
                    interceptionContext.Result = new DbQueryCommandTree(
                        queryCommand.MetadataWorkspace,
                        queryCommand.DataSpace,
                        newQuery);
                    return;
                }

                var insertCommand = interceptionContext.Result as DbInsertCommandTree;
                if (insertCommand != null)
                {
                    var column = EdmHelper.GetFactoryColumnName(insertCommand.Target.VariableType.EdmType);
                    if (!string.IsNullOrEmpty(column))
                    {
                        // Create the variable reference in order to create the property
                        var variableReference = DbExpressionBuilder.Variable(insertCommand.Target.VariableType,
                            insertCommand.Target.VariableName);
                        // Create the property to which will assign the correct value
                        var tenantProperty = DbExpressionBuilder.Property(variableReference, column);
                        // Create the set clause, object representation of sql insert command
                        var tenantSetClause =
                            DbExpressionBuilder.SetClause(tenantProperty, DbExpression.FromString("1"));

                        // Remove potential assignment of tenantId for extra safety
                        var filteredSetClauses =
                            insertCommand.SetClauses.Cast<DbSetClause>()
                                .Where(sc => ((DbPropertyExpression)sc.Property).Property.Name != column);

                        // Construct the final clauses, object representation of sql insert command values
                        var finalSetClauses =
                            new ReadOnlyCollection<DbModificationClause>
                            (new List<DbModificationClause>(filteredSetClauses) {
                            tenantSetClause
                            });

                        var newInsertCommand = new DbInsertCommandTree(
                            insertCommand.MetadataWorkspace,
                            insertCommand.DataSpace,
                            insertCommand.Target,
                            finalSetClauses,
                            insertCommand.Returning);

                        interceptionContext.Result = newInsertCommand;
                        return;
                    }
                }
            }
        }
    }
}
