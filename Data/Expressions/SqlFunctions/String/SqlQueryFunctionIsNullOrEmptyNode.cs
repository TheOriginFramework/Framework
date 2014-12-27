using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.String
{
    public class SqlQueryFunctionIsNullOrEmptyNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("IsNullOrEmpty", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = FunctionExpressionNode as MethodCallExpression;
            var expParser = SqlQueryExpressionFactory.GetNodeParser(exp, this.ModelStrategy);
            var argExp = SqlQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), this.ModelStrategy);

            expressionBuilder.Append("(");
            expressionBuilder.Append(argExp.Parse(exp.Arguments.First()));
            expressionBuilder.Append(" IS NULL) OR (LEN(");
            expressionBuilder.Append(argExp.Parse(exp.Arguments.First()));
            expressionBuilder.Append(") = 0)");

            return expressionBuilder.ToString();
        }
    }
}
