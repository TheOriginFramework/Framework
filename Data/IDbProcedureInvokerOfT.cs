using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IDbProcedureInvoker<TModel> : IDbProcedureInvoker
        where TModel : class, new()
    {
        void Invoke(TModel Model);
        void Invoke(IEnumerable<TModel> Models);
        IEnumerable<dynamic> InvokeGet(TModel Model);
    }
}
