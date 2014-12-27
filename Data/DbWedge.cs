using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    // DbWedge is a lightweight data access object for manual SQL execution service.
    public class DbWedge : DbClient
    {
        public DbWedge() : base() { }
        public DbWedge(string ConnectionString) : base(ConnectionString) { }

        public ISqlQueryExecutor GetQueryExecutor()
        {
            return this.CreateQueryExecutor();
        }
    }
}
