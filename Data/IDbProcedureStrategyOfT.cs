using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IDbProcedureStrategy<TModel> : IDbProcedureStrategy
        where TModel: class, new()
    {
        IParameterBindingInfo DefineParameter(string ParameterName, Expression<Func<TModel, object>> PropertySpecifier);
    }
}
