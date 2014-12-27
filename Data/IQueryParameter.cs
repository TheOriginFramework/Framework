using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryParameter
    {
        IQueryParameter Name(string Name);
        IQueryParameter MapToDbType(DbType Type);
        IQueryParameter ForDataLength(int Length);
        IQueryParameter AsIn();
        IQueryParameter AsOut();
        IQueryParameter AsReturnValue();
        IQueryParameter Value<T>(T ParameterValue);
    }
}
