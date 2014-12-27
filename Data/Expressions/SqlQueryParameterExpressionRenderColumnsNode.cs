using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryParameterExpressionRenderColumnsNode : SqlQueryExpressionNode
    {
        public override string Parse(System.Linq.Expressions.Expression ExpressionNode)
        {
            var paramExp = ExpressionNode as ParameterExpression;
            var paramProperties = paramExp.Type.GetProperties();
            var sb = new StringBuilder();

            foreach (var prop in paramProperties)
            {
                if (sb.Length == 0)
                    sb.Append(paramExp.Name + "." + prop.Name);
                else
                {
                    sb.Append(", ");
                    sb.Append(paramExp.Name + "." + prop.Name);
                }
            }

            return sb.ToString();
        }
    }
}
