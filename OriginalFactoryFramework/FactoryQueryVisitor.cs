using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSD.FactoryFramework
{
    public class FactoryQueryVisitor : DefaultExpressionVisitor
    {
        public override DbExpression Visit(DbScanExpression expression)
        {
            var column = EdmHelper.GetFactoryColumnName(expression.Target.ElementType);

            if (!string.IsNullOrEmpty(column))
            {
                // Get the current expression
                var dbExpression = base.Visit(expression);
                // Get the current expression binding
                var currentExpressionBinding = DbExpressionBuilder.Bind(dbExpression);
                
                //FactoryId = @Paramname_1
                // Create the variable reference in order to create the property
                var variableReferenceLeftSide = DbExpressionBuilder.Variable(currentExpressionBinding.VariableType,
                    currentExpressionBinding.VariableName);
                // Create the property based on the variable in order to apply the equality
                var tenantPropertyLeftSide = DbExpressionBuilder.Property(variableReferenceLeftSide, column);
                // Create the parameter which is an object representation of a sql parameter.
                // We have to create a parameter and not perform a direct comparison with Equal function for example
                // as this logic is cached per query and called only once
                var tenantParameterLeftSide = DbExpressionBuilder.Parameter(tenantPropertyLeftSide.Property.TypeUsage,
                    FactoryConstants.FactoryIdFilterParameterName);
                // Apply the equality between property and parameter.
                var filterExpressionLeftSide = DbExpressionBuilder.Equal(tenantPropertyLeftSide, tenantParameterLeftSide);

                //1 = @Paramname_2
                // Create the variable reference in order to create the property
                var variableReferenceRightSide = DbExpressionBuilder.Variable(currentExpressionBinding.VariableType,
                    currentExpressionBinding.VariableName);
                // Create the property based on the variable in order to apply the equality
                var tenantPropertyRightSide = DbExpression.FromInt32(1);
                // Create the parameter which is an object representation of a sql parameter.
                // We have to create a parameter and not perform a direct comparison with Equal function for example
                // as this logic is cached per query and called only once
                var tenantParameterRightSide = DbExpressionBuilder.Parameter(TypeUsage.CreateDefaultTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32)),
                    FactoryConstants.FactoryOverrideFilterParameterName);
                // Apply the equality between property and parameter.
                var filterExpressionRightSide = DbExpressionBuilder.Equal(tenantPropertyRightSide, tenantParameterRightSide);
                DbExpressionBuilder.
                var filterExpression = DbExpressionBuilder.Or(filterExpressionLeftSide, filterExpressionRightSide);

                // Apply the filtering to the initial query
                return DbExpressionBuilder.Filter(currentExpressionBinding, filterExpression);
            }

            return base.Visit(expression);
        }
    }
}
