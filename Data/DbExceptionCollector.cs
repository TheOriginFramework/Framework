using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Data.Exceptions;

namespace TOF.Framework.Data
{
    public class DbOperationExceptionCollector
    {
        private List<DbOperationException> _dbExceptions = new List<DbOperationException>();

        public void Add(DbOperationException Exception)
        {
            this._dbExceptions.Add(Exception);
        }

        public bool HasException()
        {
            return this._dbExceptions.Any();
        }

        public IEnumerable<DbOperationException> GetDbExceptions()
        {
            return this._dbExceptions;
        }
    }
}
