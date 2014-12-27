using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQuery
    {
        IQuery AsDirectQuery();
        IQuery AsTableOrViewQuery();
        IQuery AsProcedureQuery();
        IQueryParameter DefineQueryParameter<TQueryParameter>(string Name)
            where TQueryParameter: IQueryParameter;
    }
}
