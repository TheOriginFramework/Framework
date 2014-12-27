using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryOrElseExpressionNode : SqlQueryExpressionNode
    {
        public override string Parse(Expression ExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = ExpressionNode as BinaryExpression;
            var leftExp = SqlQueryExpressionFactory.GetNodeParser(exp.Left, this.ModelStrategy);
            var rightExp = SqlQueryExpressionFactory.GetNodeParser(exp.Right, this.ModelStrategy);

            expressionBuilder.Append("(");
            expressionBuilder.Append(leftExp.Parse(exp.Left));
            expressionBuilder.Append(" OR ");
            expressionBuilder.Append(rightExp.Parse(exp.Right));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
