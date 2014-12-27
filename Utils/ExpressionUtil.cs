using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Utils
{
    public class ExpressionUtil
    {
        public static string GetMapPropertyName<T>(Expression<Func<T, object>> MapPropertyExpression)
        {
            Expression expression = MapPropertyExpression.Body;

            if (expression.NodeType == ExpressionType.Constant)
            {
                if (!(expression.Type == typeof(string)))
                    throw new InvalidOperationException("ERROR_MAPPING_NAME_MUST_BE_STRING");
                else
                    return (expression as ConstantExpression).Value.ToString();
            }

            if (expression.NodeType == ExpressionType.Convert)
                expression = ((UnaryExpression)expression).Operand;

            if (expression is MemberExpression)
                return (expression as MemberExpression).Member.Name;

            return string.Empty;
        }

        public static Expression<Func<T, object>> ConvertConditionFuncToExpression<T>(Func<T, bool> Func)
        {
            return x => Func(x);
        }
    }
}
