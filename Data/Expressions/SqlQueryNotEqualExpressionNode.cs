using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryNotEqualExpressionNode : SqlQueryExpressionNode
    {
        public override string Parse(Expression ExpressionNode)
        {
            var exp = ExpressionNode as BinaryExpression;
            var expressionBuilder = new StringBuilder();
            var leftExp = SqlQueryExpressionFactory.GetNodeParser(exp.Left, this.ModelStrategy);
            var rightExp = SqlQueryExpressionFactory.GetNodeParser(exp.Right, this.ModelStrategy);
            bool leftNull = false, rightNull = false;
            
            if (exp.Left is ConstantExpression)
                leftNull = (exp.Left as ConstantExpression).Value == null;
            if (exp.Right is ConstantExpression)
                rightNull = (exp.Right as ConstantExpression).Value == null;
            
            expressionBuilder.Append("("); 
            expressionBuilder.Append(leftExp.Parse(exp.Left));

            if (leftNull || rightNull)
                expressionBuilder.Append(" IS NOT ");
            else
                expressionBuilder.Append(" <> ");

            expressionBuilder.Append(rightExp.Parse(exp.Right));            
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
