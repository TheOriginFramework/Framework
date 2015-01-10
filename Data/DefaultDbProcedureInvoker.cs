using TOF.Framework.Contracts.Exceptions;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultDbProcedureInvoker : IDbProcedureInvoker
    {
        private IDbProcedureStrategy _procedureStrategy = null;

        protected string ConnectionString { get; set; }

        public DefaultDbProcedureInvoker(IDbProcedureStrategy ProcedureStrategy)
        {
            this._procedureStrategy = ProcedureStrategy;
        }

        public void ConfigureDbConnection(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public void Invoke(params object[] Parameters)
        {
            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this.ConnectionString });
            var parameters = this._procedureStrategy.RenderParameters();

            // verify parameter count.
            if (Parameters.Length != parameters.Count())
                throw new DbOperationException(
                    "ERROR_PARAMETER_COUNT_MISMATCHED", this._procedureStrategy.GetProcedureName(), parameters);

            // assign parameter.
            for (int i = 0; i < parameters.Count(); i++)
                parameters.ElementAt(i).Value = Parameters[i];

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

        public void Invoke(IEnumerable<object[]> Parameters)
        {
            if (!Parameters.Any())
                return;

            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this.ConnectionString });
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            var parameters = this._procedureStrategy.RenderParameters();

            // verify parameter count.
            if (Parameters.First().Length != parameters.Count())
                throw new DbOperationException(
                    "ERROR_PARAMETER_COUNT_MISMATCHED", this._procedureStrategy.GetProcedureName(), parameters);

            try
            {
                executionProvider.Open();

                transactionProvider.BeginTransaction();

                foreach (var parameterItem in Parameters)
                {
                    // assign parameter.
                    for (int i = 0; i < parameters.Count(); i++)
                        parameters.ElementAt(i).Value = parameterItem[i];

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

        public IEnumerable<dynamic> InvokeGet(params object[] Parameters)
        {
            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this.ConnectionString });
            var parameters = this._procedureStrategy.RenderParameters();

            // verify parameter count.
            if (Parameters.Length != parameters.Count())
                throw new DbOperationException(
                    "ERROR_PARAMETER_COUNT_MISMATCHED", this._procedureStrategy.GetProcedureName(), parameters);

            // assign parameter.
            for (int i = 0; i < parameters.Count(); i++)
                parameters.ElementAt(i).Value = Parameters[i];

            try
            {
                executionProvider.Open();
                var reader = executionProvider.ExecuteProcedureQuery(this._procedureStrategy.GetProcedureName(), parameters);

                if (reader == null)
                    throw new InvalidOperationException("E_DATAREADER_RETURNS_NULL");

                var records = Utils.GetDataRecords(reader);
                List<dynamic> items = new List<dynamic>();

                foreach (var record in records)
                {
                    dynamic item = new ExpandoObject();
                    IDictionary<string, object> itemProps = item as IDictionary<string, object>;

                    for (int i=0; i<record.FieldCount; i++)
                    {
                        try
                        {
                            ((IDictionary<string, object>)item).Add(record.GetName(i).Trim(), record.GetValue(i));
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

        protected IEnumerable<IDataRecord> GetDataRecords(IDataReader reader)
        {
            while (reader.Read())
                yield return reader;
        }
    }
}
