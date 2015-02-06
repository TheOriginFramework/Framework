using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlQueryExecutor
    {
        // manage connection for bulk insert or update.
        // manual open and close.
        void Open();
        void Close();

        // execute query and commands.
        IEnumerable<dynamic> ExecuteQuery(ISqlQuery Query);
        IEnumerable<T> ExecuteQuery<T>(ISqlQuery Query);
        object ExecuteQueryGetScalar(ISqlQuery Query);
        T ExecuteQueryGetScalar<T>(ISqlQuery Query);
        int Execute(ISqlQuery Query);
        void Execute(IEnumerable<ISqlQuery> QueryBatch);
    }
}
