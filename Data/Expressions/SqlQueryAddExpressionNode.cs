using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryAddExpressionNode : SqlQueryExpressionNode
    {
        public override string Parse(Expression ExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = ExpressionNode as BinaryExpression;
            var leftExp = SqlQueryExpressionFactory.GetNodeParser(exp.Left, this.ModelStrategy);
            var rightExp = SqlQueryExpressionFactory.GetNodeParser(exp.Right, this.ModelStrategy);

            expressionBuilder.Append(leftExp.Parse(exp.Left));

            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    expressionBuilder.Append(" + ");
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    expressionBuilder.Append(" - ");
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    expressionBuilder.Append(" * ");
                    break;
                case ExpressionType.Divide:
                    expressionBuilder.Append(" / ");
                    break;
            }

            expressionBuilder.Append(rightExp.Parse(exp.Right));

            return expressionBuilder.ToString();
        }
    }
}
