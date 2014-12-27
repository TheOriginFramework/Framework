using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOF.Framework.Contracts.Exceptions;

namespace TOF.Framework.Data.Exceptions
{
    // TODO: implement ILogRequriedException interface 
    public class DbOperationException : FrameworkException
    {
        private string _exceptionStatement = null;
        private IEnumerable<IDbDataParameter> _exceptionParameters = null;

        public DbOperationException(string Message, string Statement, IEnumerable<IDbDataParameter> Parameters)
            : base(Message)
        {
        }

        public DbOperationException(string Message, Exception InnerException, string Statement, IEnumerable<IDbDataParameter> Parameters)
            : base(Message, InnerException)
        {
            this._exceptionStatement = Statement;
            this._exceptionParameters = Parameters;
        }

        public string GetExceptionStatement()
        {
            return this._exceptionStatement;
        }

        public IEnumerable<IDbDataParameter> GetExceptionParameters()
        {
            return this._exceptionParameters;
        }
    }
}
