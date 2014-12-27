using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IModelStrategy<TModel> : IModelStrategy
        where TModel: class, new()
    {
        IPropertyBindingInfo DefineProperty(Expression<Func<TModel, object>> PropertySpecifier);
        IDbProcedureStrategy<TModel> DefineInsertProcedure(string ProcedureName);
        IDbProcedureStrategy<TModel> DefineUpdateProcedure(string ProcedureName);
        IDbProcedureStrategy<TModel> DefineDeleteProcedure(string ProcedureName);
    }
}
