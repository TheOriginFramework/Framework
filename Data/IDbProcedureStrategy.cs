using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IDbProcedureStrategy
    {
        string GetProcedureName(); 
        IEnumerable<IDbDataParameter> RenderParameters();
        IDictionary<string, IParameterBindingInfo> GetParameterBindings();
    }
}
