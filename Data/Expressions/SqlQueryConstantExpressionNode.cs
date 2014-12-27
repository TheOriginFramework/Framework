using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryConstantExpressionNode : SqlQueryExpressionNode
    {
        public override string Parse(Expression ExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = ExpressionNode as ConstantExpression;

            if (exp.Value == null)
                expressionBuilder.Append("NULL");
            else
            {
                if (exp.Value.GetType() == typeof(string))
                {
                    expressionBuilder.Append("'");
                    expressionBuilder.Append(exp.Value.ToString());
                    expressionBuilder.Append("'");
                }
                else
                    expressionBuilder.Append(exp.Value.ToString());
            }

            return expressionBuilder.ToString();
        }
    }
}
