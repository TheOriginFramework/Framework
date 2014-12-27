using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Contracts.Exceptions;

namespace TOF.Framework.Data.Exceptions
{
    public class DbEnvironmentException : FrameworkException
    {
        public DbEnvironmentException(string Message, Exception InnerException = null)
            : base(Message, InnerException)
        {
        }
    }
}
