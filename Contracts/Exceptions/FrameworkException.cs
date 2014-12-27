using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Contracts.Exceptions
{
    public abstract class FrameworkException : Exception
    {
        public FrameworkException()
        {
        }
        public FrameworkException(string Message)
            : base(Message)
        {
        }
        public FrameworkException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }
    }
}
