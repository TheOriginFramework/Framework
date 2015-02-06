using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlExecutionProvider
    {
        void Open();
        void Close();
        int Execute(string Statement, IEnumerable<IDbDataParameter> Parameters);
        int ExecuteProcedure(string ProcedureName, IEnumerable<IDbDataParameter> Parameters);
        IDataReader ExecuteQuery(string Statement, IEnumerable<IDbDataParameter> Parameters);
        object ExecuteQueryGetScalar(string Statement, IEnumerable<IDbDataParameter> Parameters);
        IDataReader ExecuteProcedureQuery(string ProcedureName, IEnumerable<IDbDataParameter> Parameters);
        object ExecuteProcedureQueryGetScalar(string ProcedureName, IEnumerable<IDbDataParameter> Parameters);
    }
}
