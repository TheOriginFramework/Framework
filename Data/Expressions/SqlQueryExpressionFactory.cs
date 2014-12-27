using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryExpressionFactory
    {
        public static IQueryExpressionNode GetNodeParser(Expression Expression, IModelStrategy ModelStrategy)
        {
            SqlQueryExpressionNode expressionNode = null;

            switch (Expression.NodeType)
            {
                case ExpressionType.NotEqual:
                    expressionNode = new SqlQueryNotEqualExpressionNode();
                    break;

                case ExpressionType.Not:
                    expressionNode = new SqlQueryNotExpressionNode();
                    break;

                case ExpressionType.Equal:
                    expressionNode = new SqlQueryEqualExpressionNode();
                    break;

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    expressionNode = new SqlQueryGreaterThanExpressionNode();
                    break;

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    expressionNode = new SqlQueryLessThanExpressionNode();
                    break;

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    expressionNode = new SqlQueryAddExpressionNode();
                    break;

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    expressionNode = new SqlQuerySubstractExpressionNode();
                    break;

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    expressionNode = new SqlQueryMultiplyExpressionNode();
                    break;

                case ExpressionType.Divide:
                    expressionNode = new SqlQueryDivideExpressionNode();
                    break;

                case ExpressionType.MemberAccess:
                    expressionNode = new SqlQueryMemberAccessExpressionNode();
                    break;

                case ExpressionType.AndAlso:
                    expressionNode = new SqlQueryAndAlsoExpressionNode();
                    break;

                case ExpressionType.OrElse:
                    expressionNode = new SqlQueryOrElseExpressionNode();
                    break;

                case ExpressionType.Constant:
                    expressionNode = new SqlQueryConstantExpressionNode();
                    break;

                case ExpressionType.Call:
                    expressionNode = new SqlQueryCallExpressionNode();
                    break;

                case ExpressionType.Parameter:
                    expressionNode = new SqlQueryParameterExpressionNode();
                    break;

                case ExpressionType.Convert:
                    expressionNode = new SqlQueryGetMemberNameExpressionNode();
                    break;
                    
                default:
                    throw new NotSupportedException("ERROR_GIVEN_QUERY_EXPRESSION_TYPE_NOT_SUPPORTED");
            }

            expressionNode.ModelStrategy = ModelStrategy;
            return expressionNode;
        }
    }
}
