using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public class DbSchemaInitilizer
    {
        private Assembly[] _assembliesToInitSchema = null;

        public DbSchemaInitilizer()
        {
        }

        public DbSchemaInitilizer(Assembly[] AssembliesToInitSchema)
        {
            this._assembliesToInitSchema = AssembliesToInitSchema;
        }

        public void Execute()
        {

        }

        private string PrepareSchemaCreation(Assembly AssemblyToInitSchema)
        {
            var assemblyTypes = AssemblyToInitSchema.GetExportedTypes();
            var dbTypeQuery = assemblyTypes.Where(c => c.BaseType == typeof(DbClient));

            if (!dbTypeQuery.Any())
                return string.Empty;

            foreach (var dbType in dbTypeQuery)
            {
                var modelStrategies = this.GetModelStragiesFromType(dbType);

                if (!modelStrategies.Any())
                    continue;

                foreach (var strategy in modelStrategies)
                {

                }
            }

            return null; // TODO: implement PrepareSchemaCreation()
        }

        private IEnumerable<IModelStrategy> GetModelStragiesFromType(Type schemaInitType)
        {
            var client = Activator.CreateInstance(schemaInitType) as DbClient;
            var modelStrategyBuilder = client.GetModelStrategy();

            var tableTypes = schemaInitType.GetProperties(
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (!tableTypes.Any())
                yield break;

            foreach (var tableType in tableTypes)
                yield return this.GetModelStrategyFromModel(modelStrategyBuilder, tableType);
        }

        private IModelStrategy GetModelStrategyFromModel(ModelStrategyBuilder ModelStrategyBuilder, PropertyInfo TableProperty)
        {
            if (!TableProperty.PropertyType.IsGenericType)
            {
                var modelType = ModelStrategyBuilder.GetModelTypeFromProperty(TableProperty.Name);

                // if model strategy is defined, pass it into table constructor.
                return ModelStrategyBuilder.GetTableStrategy(modelType);
            }
            else
            {
                if (TableProperty.PropertyType.FullName.Contains("ITable"))
                {
                    var tableInterfaceType = TableProperty.PropertyType;

                    // get model type for this table.
                    var modelType = tableInterfaceType.GetGenericArguments().First();
                    // create default implementation.
                    var tableImplType = typeof(DefaultTable<>).MakeGenericType(modelType);

                    // if model strategy is defined, pass it into table constructor.
                    return ModelStrategyBuilder.GetTableStrategy(modelType);
                }
                else
                    return null;
            }
        }

        private string CreateSchemaPrepareStatement(IModelStrategy strategy)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("CREATE TABLE ");
            sqlBuilder.Append(strategy.GetTableName());
            sqlBuilder.Append(" (");

            // process column definitions.
            var props = strategy.GetModelPropertyBindings();
            int columnIndex = 0;

            foreach (var prop in props)
            {
                string dbName = prop.GetParameterName();
                var dbType = this.GetTypeToDbType(prop.GetPropertyInfo().PropertyType);
                string sqlType = this.MapDbTypeToSqlDbType(dbType);
                bool isString =
                    (dbType == DbType.String || dbType == DbType.StringFixedLength ||
                     dbType == DbType.AnsiString || dbType == DbType.AnsiStringFixedLength);
                int length = (!prop.GetLength().HasValue && isString) ? 50 : prop.GetLength().Value;

                sqlBuilder.Append(" " + dbName + " " + sqlType);

                if (isString)
                    sqlBuilder.Append("(" + length + ")");

                if (prop.IsKey())
                    sqlBuilder.Append("  PRIMARY KEY CLUSTERED");

                if (columnIndex < props.Count())
                    sqlBuilder.Append(", ");
            }

            sqlBuilder.Append(") ");

            return sqlBuilder.ToString();
        }

        private DbType GetTypeToDbType(Type type)
        {
            if (type == typeof(int))
                return DbType.Int32;
            else if (type == typeof(long))
                return DbType.Int64;
            else if (type == typeof(short))
                return DbType.Int16;
            else if (type == typeof(string))
                return DbType.String;
            else if (type == typeof(char))
                return DbType.StringFixedLength;
            else if (type == typeof(bool))
                return DbType.Boolean;
            else if (type == typeof(byte))
                return DbType.Byte;
            else if (type == typeof(byte[]))
                return DbType.Binary;
            else if (type == typeof(decimal))
                return DbType.Decimal;
            else if (type == typeof(float))
                return DbType.Single;
            else if (type == typeof(double))
                return DbType.Double;
            else if (type == typeof(DateTime))
                return DbType.DateTime;
            else if (type == typeof(Guid))
                return DbType.Guid;

            if (type.IsEnum)
                return DbType.Int32;

            return DbType.Object;
        }

        private string MapDbTypeToSqlDbType(DbType type)
        {
            SqlParameter p = new SqlParameter();

            try
            {
                p.DbType = type;
            }
            catch (Exception)
            {
                // do not handle.
            }

            return Enum.GetName(typeof(SqlDbType), p.SqlDbType);
        }
    }
}
