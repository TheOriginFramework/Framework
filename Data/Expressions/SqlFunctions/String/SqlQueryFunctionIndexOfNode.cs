using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.String
{
    public class SqlQueryFunctionIndexOfNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("IndexOf", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = FunctionExpressionNode as MethodCallExpression;
            var expParser = SqlQueryExpressionFactory.GetNodeParser(exp, this.ModelStrategy);
            var argExp = SqlQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), this.ModelStrategy);
            var callSourceExp = SqlQueryExpressionFactory.GetNodeParser(exp.Object, this.ModelStrategy);
            var argument = argExp.Parse(exp.Arguments.First());

            expressionBuilder.Append("CHARINDEX(");
            expressionBuilder.Append(argExp.Parse(exp.Arguments.First()));
            expressionBuilder.Append(", ");
            expressionBuilder.Append(callSourceExp.Parse(exp.Object));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
