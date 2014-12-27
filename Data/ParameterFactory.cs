using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class ParameterFactory
    {
        protected static IList<Type> ParameterTypes { get; set; }

        static ParameterFactory()
        {
            ParameterTypes = new List<Type>();


        }

        public static IQueryParameter DefineParameter<TQueryParameter>()
            where TQueryParameter: IQueryParameter
        {
            return Activator.CreateInstance<TQueryParameter>() as IQueryParameter;
        }
    }
}
