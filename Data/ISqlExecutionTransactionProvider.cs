using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlExecutionTransactionProvider
    {
        void BeginTransaction();
        void BeginTransaction(IsolationLevel Level);
        void Commit();
        void Rollback();
    }
}
