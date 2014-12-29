using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryGetMemberNameExpressionNode : SqlQueryExpressionNode
    {
        public override string Parse(Expression ExpressionNode)
        {
            var expressionBuilder = new StringBuilder();

            if (ExpressionNode is MemberExpression)
            {
                var name = (ExpressionNode as MemberExpression).Member.Name;

                if (this.ModelStrategy != null)
                {
                    var propQuery = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.GetPropertyInfo().Name == name);

                    if (propQuery.Any())
                        return propQuery.First().GetParameterName();
                    else
                        return name;
                }
                else
                    return name;
            }
            else if (ExpressionNode is UnaryExpression)
            {
                var exp = ExpressionNode as UnaryExpression;

                if (exp.Operand is MemberExpression)
                {
                    var name = (exp.Operand as MemberExpression).Member.Name;

                    if (this.ModelStrategy != null)
                    {
                        var propQuery = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.GetPropertyInfo().Name == name);

                        if (propQuery.Any())
                            return propQuery.First().GetParameterName();
                        else
                            return name;
                    }
                    else
                        return name;
                }
                else if (exp.Operand is BinaryExpression)
                {
                    var binaryExp = exp.Operand as BinaryExpression;
                    var binaryExpStringBuilder = new StringBuilder();

                    // handle left expression.
                    var leftExpNode = SqlQueryExpressionFactory.GetNodeParser(binaryExp.Left, this.ModelStrategy);
                    var rightExpNode = SqlQueryExpressionFactory.GetNodeParser(binaryExp.Right, this.ModelStrategy);

                    binaryExpStringBuilder.Append(leftExpNode.Parse(binaryExp.Left));

                    switch (binaryExp.NodeType)
                    {
                        case ExpressionType.Equal:
                            binaryExpStringBuilder.Append(" = ");
                            break;
                        case ExpressionType.NotEqual:
                            binaryExpStringBuilder.Append(" <> ");
                            break;
                        case ExpressionType.GreaterThan:
                            binaryExpStringBuilder.Append(" > ");
                            break;
                        case ExpressionType.GreaterThanOrEqual:
                            binaryExpStringBuilder.Append(" >= ");
                            break;
                        case ExpressionType.LessThan:
                            binaryExpStringBuilder.Append(" < ");
                            break;
                        case ExpressionType.LessThanOrEqual:
                            binaryExpStringBuilder.Append(" <= ");
                            break;
                        case ExpressionType.AndAlso:
                            binaryExpStringBuilder.Append(" AND ");
                            break;
                        case ExpressionType.OrElse:
                            binaryExpStringBuilder.Append(" OR ");
                            break;
                        default:
                            throw new NotSupportedException("ERROR_NOT_SUPPORTED_CONDITION_EXPRESSION_TYPE");
                    }

                    binaryExpStringBuilder.Append(rightExpNode.Parse(binaryExp.Right));

                    return binaryExpStringBuilder.ToString();
                }
                else if (exp.Operand is MethodCallExpression)
                {
                    var callExpressionNode = new SqlQueryCallExpressionNode()
                    {
                        ModelStrategy = this.ModelStrategy
                    };
                    return callExpressionNode.Parse(exp.Operand);
                }
                else if (exp.Operand is UnaryExpression)
                {
                    var unaryExpression = new SqlQueryConstantExpressionNode()
                    {
                        ModelStrategy = this.ModelStrategy
                    };
                    return unaryExpression.Parse(exp.Operand);
                }
                else //if (exp is method)
                {
                    throw new NotSupportedException("ERROR_NOT_SUPPORTED_EXPRESSION_TYPE");
                }
            }
            else
                throw new NotSupportedException("ERROR_NOT_SUPPORTED_EXPRESSION_TYPE");
        }
    }
}
