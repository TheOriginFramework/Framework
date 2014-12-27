using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions.DateTime
{
    public class SqlQueryFunctionDateTimeMonthNode : SqlQueryExpressionNode, IQueryFunctionNode
    {
        public bool CheckForHandle(string FunctionName)
        {
            return string.Equals("Month", FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string Parse(Expression FunctionExpressionNode)
        {
            var expressionBuilder = new StringBuilder();
            var expParser = SqlQueryExpressionFactory.GetNodeParser(FunctionExpressionNode, this.ModelStrategy);

            expressionBuilder.Append("DATEPART(MONTH, ");
            expressionBuilder.Append(expParser.Parse(FunctionExpressionNode));
            expressionBuilder.Append(")");

            return expressionBuilder.ToString();
        }
    }
}
