using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data.Expressions.SqlFunctions
{
    public class SqlFunctionsLoader
    {
        private static IEnumerable<IQueryFunctionNode> _loadedFunctions = null;

        public static IEnumerable<IQueryFunctionNode> Load()
        {
            if (_loadedFunctions == null)
            {
                List<IQueryFunctionNode> functions = new List<IQueryFunctionNode>();
                Assembly thisAssembly = typeof(SqlFunctionsLoader).Assembly;
                var functionTypes = thisAssembly.GetTypes().Where(
                    c => c.FindInterfaces(new TypeFilter((t, o) => t == typeof(IQueryFunctionNode)), null).Any());

                foreach (var type in functionTypes)
                    functions.Add(Activator.CreateInstance(type) as IQueryFunctionNode);

                _loadedFunctions = functions;
            }

            return _loadedFunctions;
        }
    }
}
