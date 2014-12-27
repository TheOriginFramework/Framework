using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IDbProcedureInvoker
    {
        void Invoke(params object[] Parameters);
        void Invoke(IEnumerable<object[]> Parameters);
        IEnumerable<dynamic> InvokeGet(params object[] Parameters);
    }
}
