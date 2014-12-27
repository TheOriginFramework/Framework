using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Contracts.Exceptions;

namespace TOF.Framework.Data.Exceptions
{
    // TODO: implement ILogRequriedException interface.
    public class DbMultipleOperationsException : FrameworkException
    {
        private IEnumerable<DbOperationException> _operationExceptions = null;

        public DbMultipleOperationsException(IEnumerable<DbOperationException> OperationExceptions)
            : base("ERROR_MULTI_OPERATIONS_OCCUR_EXCEPTION")
        {
            this._operationExceptions = OperationExceptions;
        }

        public IEnumerable<DbOperationException> GetOperationExceptions()
        {
            return this._operationExceptions;
        }
    }
}
