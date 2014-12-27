using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IDbProcedure
    {
        void PopulateParams<TModel>(TModel Model);
        void Invoke();
        TReturn GetReturnValue<TReturn>();
    }
}
