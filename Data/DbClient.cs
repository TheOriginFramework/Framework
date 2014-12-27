using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TOF.Framework.Configuration;
using TOF.Framework.Data.Annotations;
using System.Data;

namespace TOF.Framework.Data
{
    public abstract class DbClient : IDisposable
    {
        private ModelStrategyBuilder _dbModelStrategyBuilder = null;
        private string _connectionString = null;
        
        public DbClient()
        {
            this._connectionString = Utils.GetConnectionString();
            this._dbModelStrategyBuilder = new ModelStrategyBuilder();

            this.DefiningModelStrategies(this._dbModelStrategyBuilder);
            this.Initialize();
        }

        public DbClient(string ConnectionString)
        {
            this._connectionString = ConnectionString;
            this._dbModelStrategyBuilder = new ModelStrategyBuilder();

            this.DefiningModelStrategies(this._dbModelStrategyBuilder);
            this.Initialize();
        }

        protected virtual void DefiningModelStrategies(ModelStrategyBuilder builder)
        {
            // implement by child service.
        }

        protected IDbProcedureInvoker Procedure(string ProcedureName)
        {
            var invoker = new DefaultDbProcedureInvoker(this._dbModelStrategyBuilder.GetProcedureStrategy(ProcedureName));
            invoker.ConfigureDbConnection(this._connectionString);
            return invoker;
        }

        protected IDbProcedureInvoker<TModel> Procedure<TModel>(string ProcedureName)
            where TModel : class, new()
        {
            var invoker = new DefaultDbProcedureInvoker<TModel>(this._dbModelStrategyBuilder.GetProcedureStrategy(ProcedureName));
            invoker.ConfigureDbConnection(this._connectionString);
            return invoker;
        }

        protected ISqlQueryExecutor CreateQueryExecutor()
        {
            return new DefaultSqlQueryExecutor(this._connectionString);
        }

        public void Dispose()
        {
            this._dbModelStrategyBuilder = null;
        }

        internal ModelStrategyBuilder GetModelStrategy()
        {
            return this._dbModelStrategyBuilder;
        }

        private void Initialize()
        {
            var tableProps = this.GetType().GetProperties(
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var tableProp in tableProps)
            {
                if (!tableProp.PropertyType.IsGenericType)
                {
                    var modelType = this._dbModelStrategyBuilder.GetModelTypeFromProperty(tableProp.Name);

                    // if model strategy is defined, pass it into table constructor.
                    IModelStrategy modelStrategy = this._dbModelStrategyBuilder.GetTableStrategy(modelType);

                    // auto-create default model strategy.
                    if (modelStrategy == null)
                        modelStrategy = this._dbModelStrategyBuilder.Table(modelType);
                    
                    // detect if model has data annotations.
                    this.DetectAndConfigureModelStrategy(modelStrategy);

                    ITable table = null;

                    // set instance to property.
                    if (modelStrategy != null)
                        table = Activator.CreateInstance(typeof(ITable), modelStrategy) as ITable;
                    else
                        table = Activator.CreateInstance(typeof(ITable)) as ITable;

                    // update connection string.
                    table.ConfigureDbConnection(this._connectionString);
                    tableProp.SetValue(this, table, null);
                }
                else
                {
                    if (tableProp.PropertyType.FullName.Contains("ITable"))
                    {
                        var tableInterfaceType = tableProp.PropertyType;

                        // get model type for this table.
                        var modelType = tableInterfaceType.GetGenericArguments().First();
                        // create default implementation.
                        var tableImplType = typeof(DefaultTable<>).MakeGenericType(modelType);

                        // if model strategy is defined, pass it into table constructor.
                        IModelStrategy modelStrategy = this._dbModelStrategyBuilder.GetTableStrategy(modelType);

                        // set instance to property.
                        if (modelStrategy != null)
                            this.DetectAndConfigureModelStrategy(modelStrategy);
                        else
                        {
                            modelStrategy = this._dbModelStrategyBuilder.Table(modelType);
                            this.DetectAndConfigureModelStrategy(modelStrategy);
                        }

                        object tableImpl = Activator.CreateInstance(tableImplType, modelStrategy);

                        // update connection string.
                        tableImpl.GetType().GetMethod("ConfigureDbConnection")
                            .Invoke(tableImpl, new object[] { this._connectionString });

                        tableProp.SetValue(this, tableImpl, null);
                    }
                }
            }
        }
 
        private IModelStrategy DetectAndConfigureModelStrategy(IModelStrategy strategy)
        {
            Type modelType = strategy.GetModelType();
            var properties = modelType.GetProperties().Where(p => p.DeclaringType != typeof(object));

            TableAttribute attrTable = 
                this.GetClassLevelDataAnnotation<TableAttribute>(modelType);

            if (attrTable != null)
                strategy.ChangeTableName(attrTable.TableName);

            foreach (var property in properties)
            {
                IPropertyBindingInfo bindingPropertyInfo = strategy.DefinePropertyExact(property);

                AllowNullAttribute attrAllowNull = 
                    this.GetPropertyLevelDataAnnotation<AllowNullAttribute>(property);

                if (attrAllowNull != null && attrAllowNull.AllowNull)
                    bindingPropertyInfo = bindingPropertyInfo.AllowNull();

                KeyAttribute attrKey =
                    this.GetPropertyLevelDataAnnotation<KeyAttribute>(property);

                if (attrKey != null)
                {
                    if (bindingPropertyInfo.IsAllowNull())
                        throw new InvalidOperationException("E_KEY_ATTRIBUTE_NOT_ALLOW_NULL");

                    bindingPropertyInfo = bindingPropertyInfo.AsKey();
                }

                DataColumnNameAttribute attrDataColumnName =
                    this.GetPropertyLevelDataAnnotation<DataColumnNameAttribute>(property);

                if (attrDataColumnName != null)
                    bindingPropertyInfo = bindingPropertyInfo.DbName(attrDataColumnName.DataColumnName);            

                DbTypeAttribute attrDbType =
                    this.GetPropertyLevelDataAnnotation<DbTypeAttribute>(property);

                if (attrDbType != null)
                    bindingPropertyInfo = bindingPropertyInfo.MapDbType(attrDbType.DbType);

                if (property.PropertyType == typeof(int) || 
                    property.PropertyType == typeof(long) ||
                    property.PropertyType == typeof(short))
                {
                    AutoIncrementalAttribute attrAutoIncremental = 
                        this.GetPropertyLevelDataAnnotation<AutoIncrementalAttribute>(property);

                    if (attrAutoIncremental != null)
                        bindingPropertyInfo = bindingPropertyInfo.AsIncremental();
                }
                else if (
                    property.PropertyType == typeof(Guid) || 
                    property.PropertyType == typeof(DateTime) ||
                    bindingPropertyInfo.GetMapDbType() == DbType.Guid ||
                    bindingPropertyInfo.GetMapDbType() == DbType.DateTime)
                {
                    AutoGeneratedAttribute attrAutoGenerated = 
                        this.GetPropertyLevelDataAnnotation<AutoGeneratedAttribute>(property);

                    if (attrAutoGenerated != null)
                        bindingPropertyInfo = bindingPropertyInfo.AsAutoGenerated();
                }
                
            }

            return strategy;
        }

        private TDataAnnotationAttribute GetClassLevelDataAnnotation<TDataAnnotationAttribute>(Type Type)
            where TDataAnnotationAttribute : Attribute
        {
            TDataAnnotationAttribute dataAnnotationAttribute =
                Type.GetCustomAttribute<TDataAnnotationAttribute>(true);

            return dataAnnotationAttribute;
        }

        private TDataAnnotationAttribute GetPropertyLevelDataAnnotation<TDataAnnotationAttribute>(PropertyInfo Property)
            where TDataAnnotationAttribute : Attribute
        {
            TDataAnnotationAttribute dataAnnotationAttribute =
                Property.GetCustomAttribute<TDataAnnotationAttribute>(true);

            return dataAnnotationAttribute;
        }
    }
}
