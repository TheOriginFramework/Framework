using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryNewTypeRenderColumnNode
    {
        public IModelStrategy ModelStrategy { get; private set; }

        public SqlQueryNewTypeRenderColumnNode(IModelStrategy ModelStrategy)
        {
            this.ModelStrategy = ModelStrategy;
        }

        public IDictionary<string, string> Parse(Expression ExpressionNode)
        {
            Dictionary<string, string> PropertyMap = new Dictionary<string, string>();

            if (ExpressionNode is NewExpression)
            {
                var newExp = ExpressionNode as NewExpression;

                for (int i = 0; i < newExp.Members.Count; i++)
                {
                    SqlQueryExpressionNode memberAccessNode = new SqlQueryMemberAccessExpressionNode(false);
                    memberAccessNode.ModelStrategy = this.ModelStrategy;
                    PropertyMap.Add(newExp.Members[i].Name, memberAccessNode.Parse(newExp.Arguments[i]));
                }
            }
            else if (ExpressionNode is UnaryExpression) // example: Convert(c.UnitPrice)
            {
                SqlQueryGetMemberNameExpressionNode getMemberNameNode = new SqlQueryGetMemberNameExpressionNode();
                getMemberNameNode.ModelStrategy = this.ModelStrategy;
                string name = getMemberNameNode.Parse(ExpressionNode);
                PropertyMap.Add(name, name);
            }

            return PropertyMap;
        }
    }
}
