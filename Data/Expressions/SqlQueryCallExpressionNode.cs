using TOF.Framework.Data.Expressions.SqlFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryCallExpressionNode : SqlQueryExpressionNode
    {
        private static IEnumerable<IQueryFunctionNode> CallExpressionNodes = null;

        static SqlQueryCallExpressionNode()
        {
            CallExpressionNodes = SqlFunctionsLoader.Load();
        }

        public override string Parse(Expression ExpressionNode)
        {
            var expressionBuilder = new StringBuilder();            
            var found = false;
            string methodName = null;

            if (ExpressionNode is MethodCallExpression)
                methodName = (ExpressionNode as MethodCallExpression).Method.Name;
            else
                methodName = (ExpressionNode as MemberExpression).Member.Name;

            expressionBuilder.Append("(");

            foreach (var functionNode in CallExpressionNodes)
            {
                if (functionNode.CheckForHandle(methodName))
                {
                    (functionNode as SqlQueryExpressionNode).ModelStrategy = this.ModelStrategy;
                    expressionBuilder.Append(functionNode.Parse(ExpressionNode));
                    found = true;
                }
            }

            if (!found)
                throw new NotSupportedException();

            expressionBuilder.Append(")");
            
            return expressionBuilder.ToString();
        }
    }
}
