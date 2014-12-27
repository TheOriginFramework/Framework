using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class StartupAttribute : Attribute
    {
        public Type StartupType { get; private set; }
        public string StartupMethod { get; private set; }

        public StartupAttribute(Type StartupType)
        {
            if (StartupType == null)
                throw new ArgumentNullException("ERROR_STARTUP_TYPE_NOT_FOUND");
            if (!StartupType.GetInterfaces().Any(c => c == typeof(IStartup)))
                throw new ArgumentNullException("ERROR_STARTUP_TYPE_IS_INVALID");

            this.StartupType = StartupType;
        }
    }
}
