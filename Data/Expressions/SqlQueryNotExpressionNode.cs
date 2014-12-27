using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryNotExpressionNode : SqlQueryExpressionNode
    {
        public override string Parse(Expression ExpressionNode)
        {
            var exp = ExpressionNode as UnaryExpression;
            var expressionBuilder = new StringBuilder();
            var opExpressionParser = SqlQueryExpressionFactory.GetNodeParser(exp.Operand, this.ModelStrategy);
            expressionBuilder.Append("( NOT (");
            expressionBuilder.Append(opExpressionParser.Parse(exp.Operand));
            expressionBuilder.Append(") )");

            return expressionBuilder.ToString();
        }
    }
}
