using TOF.Framework.Data.Expressions.SqlFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryMemberAccessExpressionNode : SqlQueryExpressionNode
    {
        private static IEnumerable<IQueryFunctionNode> SupportedFunctionNodes = null;
        private bool _requireToRenderAlias = true;

        static SqlQueryMemberAccessExpressionNode()
        {
            SupportedFunctionNodes = SqlFunctionsLoader.Load();
        }

        public SqlQueryMemberAccessExpressionNode(bool RequireToRenderAlias = true)
        {
            this._requireToRenderAlias = RequireToRenderAlias;
        }

        public override string Parse(Expression ExpressionNode)
        {
            var exp = ExpressionNode as MemberExpression;
            var expressionBuilder = new StringBuilder();

            if (exp.Member.MemberType == MemberTypes.Property)
            {
                if (exp.Expression == null)
                {
                    var callExpression = new SqlQueryCallExpressionNode()
                    {
                        ModelStrategy = this.ModelStrategy
                    };

                    expressionBuilder.Append(callExpression.Parse(exp));
                }
                else
                {
                    var expParser = SqlQueryExpressionFactory.GetNodeParser(exp.Expression, this.ModelStrategy);
                    bool handled = false;

                    foreach (var functionNode in SupportedFunctionNodes)
                    {
                        if (functionNode.CheckForHandle(exp.Member.Name))
                        {
                            ((SqlQueryExpressionNode)functionNode).ModelStrategy = this.ModelStrategy;
                            expressionBuilder.Append(functionNode.Parse(exp.Expression));
                            handled = true;
                            break;
                        }
                    }

                    if (!handled)
                    {
                        // get alias type.
                        Type t = exp.Expression.Type;

                        // check this is field (must render as parameter, not field)
                        // solution referenced from 
                        // http://stackoverflow.com/questions/6635678/how-to-recognize-a-lambda-memberexpression-of-type-field-reference
                        bool isQueryParameterProperty =
                            exp.Member is PropertyInfo && !(exp.Expression.NodeType == ExpressionType.Parameter);
                        var propQuery = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.GetPropertyInfo().Name == exp.Member.Name);

                        if (propQuery.Any())
                        {
                            if (!isQueryParameterProperty)
                            {
                                if (this._requireToRenderAlias)
                                {
                                    expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                                    expressionBuilder.Append(".");
                                }

                                expressionBuilder.Append(propQuery.First().GetParameterName());
                            }
                            else
                                expressionBuilder.Append(this.ExecuteLambdaExpression(exp.Expression as MemberExpression));
                        }
                        else
                        {
                            if (!isQueryParameterProperty)
                            {
                                if (this._requireToRenderAlias)
                                {
                                    expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                                    expressionBuilder.Append(".");
                                }

                                expressionBuilder.Append(exp.Member.Name);
                            }
                            else
                                expressionBuilder.Append(this.ExecuteLambdaExpression(exp.Expression as MemberExpression));
                        }
                    }
                }
            }
            else if (exp.Member.MemberType == MemberTypes.Field)
            {
                if (exp.Type == typeof(string))
                {
                    object v = Expression.Lambda(exp).Compile().DynamicInvoke();

                    expressionBuilder.Append("'");
                    expressionBuilder.Append(v ?? string.Empty);
                    expressionBuilder.Append("'");
                }
                else if (exp.Type == typeof(DateTime))
                {
                    expressionBuilder.Append("'");
                    expressionBuilder.Append(Convert.ToDateTime(Expression.Lambda(exp).Compile().DynamicInvoke()).ToString("yyyy/M/d H:m:s"));
                    expressionBuilder.Append("'");
                }
                else
                    expressionBuilder.Append(Expression.Lambda(exp).Compile().DynamicInvoke().ToString());
            }
            else
            {
                var propQuery = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.GetPropertyInfo().Name == exp.Member.Name);

                if (propQuery.Any())
                {
                    if (this._requireToRenderAlias)
                    {
                        expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                        expressionBuilder.Append(".");
                    }

                    expressionBuilder.Append(propQuery.First().GetParameterName());
                }
                else
                {
                    if (this._requireToRenderAlias)
                    {
                        expressionBuilder.Append(this.ModelStrategy.GetTableAlias());
                        expressionBuilder.Append(".");
                    }

                    expressionBuilder.Append(exp.Member.Name);
                }
            }

            return expressionBuilder.ToString();
        }

        private string ExecuteLambdaExpression(MemberExpression LambdaExpression)
        {
            var expressionBuilder = new StringBuilder();

            // compile expression and invoke it to get value.
            // solution found:
            // http://stackoverflow.com/questions/3457558/getting-values-from-expressiontrees
            object v = Expression.Lambda(LambdaExpression.Expression).Compile().DynamicInvoke();
            // use reflection to get mapped property value.
            PropertyInfo expMemberProperty = LambdaExpression.Member as PropertyInfo;
            object pv = expMemberProperty.GetValue(v, null);

            if (expMemberProperty.PropertyType == typeof(DateTime))
            {
                expressionBuilder.Append("'");
                expressionBuilder.Append(
                    Convert.ToDateTime(pv).ToString("yyyy/MM/dd HH:mm:ss"));
                expressionBuilder.Append("'");
            }
            else if (expMemberProperty.PropertyType == typeof(Guid))
            {
                expressionBuilder.Append("'");
                expressionBuilder.Append(pv.ToString());
                expressionBuilder.Append("'");
            }
            else if (expMemberProperty.PropertyType.IsValueType)
            {
                expressionBuilder.Append(pv.ToString());
            }
            else
            {
                expressionBuilder.Append("'");
                expressionBuilder.Append(pv.ToString());
                expressionBuilder.Append("'");
            }

            return expressionBuilder.ToString();
        }
    }
}
