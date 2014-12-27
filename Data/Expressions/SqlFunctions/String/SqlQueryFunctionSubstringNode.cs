using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.String
{
    public class SqlQueryFunctionSubstringNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("Substring", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = FunctionExpressionNode as MethodCallExpression;
            var expParser = SqlQueryExpressionFactory.GetNodeParser(exp.Object, this.ModelStrategy);
            var argExp = SqlQueryExpressionFactory.GetNodeParser(exp.Arguments.First(), this.ModelStrategy);
            int startIndex = 0, length = 0;

            var arguments = exp.Arguments;

            if (arguments.Count() == 1)
                length = Convert.ToInt32(argExp.Parse(exp.Arguments.ElementAt(0)));
            else
            {
                startIndex = Convert.ToInt32(argExp.Parse(exp.Arguments.ElementAt(0)));
                length = Convert.ToInt32(argExp.Parse(exp.Arguments.ElementAt(1)));
            }

            startIndex++; // fix different of T-SQL (start from 1) and CLR (start from 0).

            expressionBuilder.Append("SUBSTRING(");
            expressionBuilder.Append(expParser.Parse(exp.Object));
            expressionBuilder.Append(string.Format(", {0}, {1}", startIndex, length));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
