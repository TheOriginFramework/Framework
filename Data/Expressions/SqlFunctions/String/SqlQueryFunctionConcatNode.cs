using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.String
{
    public class SqlQueryFunctionConcatNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("Concat", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = FunctionExpressionNode as MethodCallExpression;
            var expParser = SqlQueryExpressionFactory.GetNodeParser(exp, this.ModelStrategy);
            var argExp = SqlQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), this.ModelStrategy);

            var arguments = exp.Arguments;
            bool firstarg = true;

            expressionBuilder.Append("CONCAT(");

            foreach (var argument in arguments)
            {
                if (!firstarg)
                    expressionBuilder.Append(", ");

                expressionBuilder.Append(argExp.Parse(argument));
            }

            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
