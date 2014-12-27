using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryNewExpressionRenderColumnsNode : SqlQueryExpressionNode
    {
        private bool _convertPropertyNameToDbName = true;

        public SqlQueryNewExpressionRenderColumnsNode(bool ConvertPropertyNameToDbName = true)
        {
            this._convertPropertyNameToDbName = ConvertPropertyNameToDbName;
        }

        public override string Parse(Expression ExpressionNode)
        {
            var sqlsb = new StringBuilder();
            var newExp = ExpressionNode as NewExpression;

            if (this._convertPropertyNameToDbName)
            {
                foreach (var memberAccessItem in newExp.Arguments)
                {
                    var memberAccessNode = SqlQueryExpressionFactory.GetNodeParser(memberAccessItem, this.ModelStrategy);

                    if (sqlsb.Length == 0)
                        sqlsb.Append(memberAccessNode.Parse(memberAccessItem));
                    else
                    {
                        sqlsb.Append(", ");
                        sqlsb.Append(memberAccessNode.Parse(memberAccessItem));
                    }
                }
            }
            else
            {
                foreach (var memberAccessItem in newExp.Arguments)
                {
                    SqlQueryExpressionNode memberAccessNode = new SqlQueryMemberAccessExpressionNode();
                    memberAccessNode.ModelStrategy = this.ModelStrategy;

                    if (sqlsb.Length == 0)
                        sqlsb.Append(memberAccessNode.Parse(memberAccessItem));
                    else
                    {
                        sqlsb.Append(", ");
                        sqlsb.Append(memberAccessNode.Parse(memberAccessItem));
                    }
                }
            }

            return sqlsb.ToString();
        }
    }
}
