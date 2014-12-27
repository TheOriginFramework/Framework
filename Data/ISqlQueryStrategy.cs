using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlQueryStrategy
    {
        string RenderQuery();
        IEnumerable<IDbDataParameter> RenderParameters();
    }
}
