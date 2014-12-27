using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.String
{
    public class SqlQueryFunctionEndsWithNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("EndsWith", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = FunctionExpressionNode as MethodCallExpression;

            if (exp.Object == null)
                throw new NotSupportedException();
            else
            {
                var expParser = SqlQueryExpressionFactory.GetNodeParser(exp, this.ModelStrategy);
                var argExp = SqlQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), this.ModelStrategy);
                var callSourceExp = SqlQueryExpressionFactory.GetNodeParser(exp.Object, this.ModelStrategy);
                var argument = argExp.Parse(exp.Arguments.First());

                expressionBuilder.Append(callSourceExp.Parse(exp.Object));
                expressionBuilder.Append(" LIKE '%");
                expressionBuilder.Append(argument.Substring(1));
                expressionBuilder.Append(" ");
            }

            return expressionBuilder.ToString();
        }
    }
}
