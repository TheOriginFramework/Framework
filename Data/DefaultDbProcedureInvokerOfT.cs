using TOF.Framework.Contracts.Exceptions;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.DependencyInjection;
using TOF.Framework.Utils;
using TOF.Framework.Utils.TypeConverters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultDbProcedureInvoker<TModel> : DefaultDbProcedureInvoker, IDbProcedureInvoker<TModel>
        where TModel : class, new()
    {
        private IDbProcedureStrategy _procedureStrategy = null;

        public DefaultDbProcedureInvoker(IDbProcedureStrategy ProcedureStrategy) : base(ProcedureStrategy)
        {
            this._procedureStrategy = ProcedureStrategy;
        }
        
        public void Invoke(TModel Model)
        {
            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this.ConnectionString });
            var parameters = this._procedureStrategy.RenderParameters();
            parameters = this.BindingModelPropertyToParameter(Model, this._procedureStrategy.GetParameterBindings(), parameters);

            try
            {
                executionProvider.Open();

                if (executionProvider.ExecuteProcedure(this._procedureStrategy.GetProcedureName(), parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_PROCEDURE_EXECUTION_FAILED", this._procedureStrategy.GetProcedureName(), parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_PROCEDURE_EXECUTION_FAILED", exception, this._procedureStrategy.GetProcedureName(), parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        public void Invoke(IEnumerable<TModel> Models)
        {
            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this.ConnectionString });
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            var parameters = this._procedureStrategy.RenderParameters();

            try
            {
                executionProvider.Open();

                transactionProvider.BeginTransaction();

                foreach (var model in Models)
                {
                    parameters = this.BindingModelPropertyToParameter(model, this._procedureStrategy.GetParameterBindings(), parameters);

                    if (executionProvider.ExecuteProcedure(this._procedureStrategy.GetProcedureName(), parameters) == 0)
                    {
                        transactionProvider.Rollback();

                        throw new DbOperationException(
                            "ERROR_SQL_PROCEDURE_EXECUTION_FAILED", this._procedureStrategy.GetProcedureName(), parameters);
                    }
                }

                transactionProvider.Commit();
            }
            catch (Exception exception)
            {
                transactionProvider.Rollback();

                throw new DbOperationException(
                    "ERROR_SQL_PROCEDURE_EXECUTION_FAILED", exception, this._procedureStrategy.GetProcedureName(), parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        public IEnumerable<dynamic> InvokeGet(TModel Model)
        {
            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this.ConnectionString });
            var parameters = this._procedureStrategy.RenderParameters();
            parameters = this.BindingModelPropertyToParameter(Model, this._procedureStrategy.GetParameterBindings(), parameters);

            try
            {
                executionProvider.Open();
                IDataReader reader = executionProvider.ExecuteProcedureGetReader(this._procedureStrategy.GetProcedureName(), parameters);

                List<dynamic> items = new List<dynamic>();
                var records = this.GetDataRecords(reader);

                foreach (var record in records)
                {
                    dynamic item = new ExpandoObject();
                    IDictionary<string, object> itemProps = item as IDictionary<string, object>;

                    for (int i = 0; i < record.FieldCount; i++)
                    {
                        try
                        {
                            ((IDictionary<string, object>)item).Add(record.GetName(i).Trim(), reader.GetValue(i));
                        }
                        catch (IndexOutOfRangeException)
                        {
                            ((IDictionary<string, object>)item).Add(record.GetName(i).Trim(), DBNull.Value);
                        }
                    }

                    items.Add(item);
                }

                reader.Close();
                return items;
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_PROCEDURE_EXECUTION_FAILED", exception, this._procedureStrategy.GetProcedureName(), parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }
        
        private IEnumerable<IDbDataParameter> BindingModelPropertyToParameter(
            TModel Model, IDictionary<string, IParameterBindingInfo> ParameterBindingInfo, IEnumerable<IDbDataParameter> Parameters)
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>(Parameters);

            foreach (var Parameter in parameters)
            {
                var propBindingQuery = ParameterBindingInfo.Where(c => c.Key == Parameter.ParameterName);

                if (propBindingQuery.Any())
                {
                    var PropertyBindingInfo = propBindingQuery.First();
                    PropertyInfo modelProperty = PropertyBindingInfo.Value.GetPropertyInfo();
                    object value = modelProperty.GetValue(Model, null);

                    if (value != null)
                    {
                        ITypeConverter converter = TypeConverterFactory.GetConvertType(modelProperty.PropertyType);

                        if (converter != null)
                        {
                            if (converter is EnumConverter)
                                Parameter.Value =
                                    (converter as EnumConverter).Convert(modelProperty.PropertyType, value);
                            else
                                Parameter.Value = converter.Convert(value);
                        }
                        else
                            Parameter.Value = value;
                    }
                    else
                    {
                        if (PropertyBindingInfo.Value.IsAllowNull())
                            Parameter.Value = DBNull.Value;
                        else
                        {
                            if (PropertyBindingInfo.Value.GetPropertyInfo().PropertyType.IsValueType)
                            {
                                Parameter.Value = Activator.CreateInstance(
                                    PropertyBindingInfo.Value.GetPropertyInfo().PropertyType);
                            }
                            else
                                Parameter.Value = string.Empty;
                        }
                    }
                }
            }

            return parameters;
        }
    }
}
