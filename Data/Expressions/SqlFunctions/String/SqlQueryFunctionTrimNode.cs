using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.String
{
    public class SqlQueryFunctionTrimNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("Trim", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var exp = FunctionExpressionNode as MethodCallExpression;
            var expParser = SqlQueryExpressionFactory.GetNodeParser(exp.Object, this.ModelStrategy);

            expressionBuilder.Append("LTRIM(RTRIM(");
            expressionBuilder.Append(expParser.Parse(exp.Object));
            expressionBuilder.Append("))");

            return expressionBuilder.ToString();
        }
    }
}
