using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IModelStrategy
    {
        Type GetModelType();
        string GetTableName();
        string GetTableAlias();
        void ChangeTableName(string TableName);
        void ChangeTableAlias(string TableAlias);
        IEnumerable<IPropertyBindingInfo> GetModelPropertyBindings();
        IDbProcedureStrategy GetInsertProcedure();
        IDbProcedureStrategy GetUpdateProcedure();
        IDbProcedureStrategy GetDeleteProcedure();
        IPropertyBindingInfo DefineProperty<T>(Expression<Func<T, object>> PropertySpecifier);
        IPropertyBindingInfo DefinePropertyExact(PropertyInfo Property);
    }
}
