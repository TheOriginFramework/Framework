﻿using TOF.Framework.DependencyInjection;
using TOF.Framework.Utils;
using TOF.Framework.Utils.TypeConverters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultSqlQueryExecutor : ISqlQueryExecutor
    {
        private string _connectionString = null;
        private bool _manageManually = false;
        private ISqlExecutionProvider _sqlExecutionProvider = null;
        private ISqlExecutionTransactionProvider _sqlTransactionProvider = null;
        internal class DbConfiguration
        {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
        }


        public DefaultSqlQueryExecutor()
        {
            this._connectionString = "";
            this.Initialize();
        }

        public DefaultSqlQueryExecutor(string ConnectionString)
        {
            this._connectionString = ConnectionString;
            this.Initialize();
        }

        private void Initialize()
        {
            this._sqlExecutionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this._connectionString });
            this._sqlTransactionProvider = this._sqlExecutionProvider as ISqlExecutionTransactionProvider;
        }

        public IEnumerable<T> ExecuteQuery<T>(ISqlQuery Query)
        {
            if (!this._manageManually)
                this._sqlExecutionProvider.Open();
            
            List<T> items = new List<T>();
            var reader = this._sqlExecutionProvider.ExecuteQuery(Query.GetSqlStatement(), Query.GetParameters());

            if (reader == null)
                throw new InvalidOperationException("E_READER_RETURNS_NULL");

            var records = Utils.GetDataRecords(reader);

            foreach (var record in records)
                items.Add(this.BindingDataRecordToModel<T>(record));

            reader.Close();

            if (!this._manageManually)
                this._sqlExecutionProvider.Close();

            return items;
        }

        public IEnumerable<dynamic> ExecuteQuery(ISqlQuery Query)
        {
            if (!this._manageManually)
                this._sqlExecutionProvider.Open();
            
            List<dynamic> items = new List<dynamic>();
            var reader = this._sqlExecutionProvider.ExecuteQuery(Query.GetSqlStatement(), Query.GetParameters());

            if (reader == null)
                throw new InvalidOperationException("E_DATAREADER_RETURNS_NULL");

            var records = Utils.GetDataRecords(reader);

            foreach (var record in records)
                items.Add(this.BindingDataRecordToModelDynamic(record));

            reader.Close();
            
            if (!this._manageManually)
                this._sqlExecutionProvider.Close();

            return items;
        }

        public object ExecuteQueryGetScalar(ISqlQuery Query)
        {
            if (!this._manageManually)
                this._sqlExecutionProvider.Open();

            object result = this._sqlExecutionProvider.ExecuteQueryGetScalar(
                Query.GetSqlStatement(), Query.GetParameters());
            
            if (!this._manageManually)
                this._sqlExecutionProvider.Close();

            return result;
        }

        public T ExecuteQueryGetScalar<T>(ISqlQuery Query)
        {
            object result = this.ExecuteQueryGetScalar(Query);

            if (result == null || result == DBNull.Value)
                return default(T);
            else
                return (T)result;
        }

        public int Execute(ISqlQuery Query)
        {
            if (!this._manageManually)
                this._sqlExecutionProvider.Open();

            try
            {
                int rows = this._sqlExecutionProvider.Execute(Query.GetSqlStatement(), Query.GetParameters());
                return rows;
            }
            finally
            {
                if (!this._manageManually)
                    this._sqlExecutionProvider.Close();
            }
        }

        public void Execute(IEnumerable<ISqlQuery> QueryBatch)
        {
            if (!this._manageManually)
                this._sqlExecutionProvider.Open();
            this._sqlTransactionProvider.BeginTransaction();

            try
            {
                foreach (var query in QueryBatch)
                    this._sqlExecutionProvider.Execute(query.GetSqlStatement(), query.GetParameters());

                this._sqlTransactionProvider.Commit();
            }
            catch (Exception)
            {
                this._sqlTransactionProvider.Rollback();
                throw;
            }
            finally
            {
                if (!this._manageManually)
                    this._sqlExecutionProvider.Close();
            }
        }

        private IEnumerable<IDataRecord> GetDataRecords(IDataReader reader)
        {
            while (reader.Read())
                yield return reader;
        }

        private TModel BindingDataRecordToModel<TModel>(IDataRecord Record)
        {
            if (typeof(TModel).GetInterface("IDynamicMetaObjectProvider") != null)
            {
                // handle with "dynamic".
                dynamic item = new ExpandoObject();
                var d = ((IDictionary<string, object>)item);

                for (int i = 0; i < Record.FieldCount; i++)
                {
                    ((IDictionary<string, object>)item).Add(
                        Record.GetName(i).Trim(), Record.GetValue(i));
                }

                return item;
            }
            else
            {
                // assign result to property.
                TModel item = (TModel)Activator.CreateInstance(typeof(TModel));

                for (int i = 0; i < Record.FieldCount; i++)
                {
                    var propInfo = typeof(TModel).GetProperty(Record.GetName(i));

                    if (propInfo == null)
                        continue;

                    object value = Record.GetValue(i);

                    if (value != null)
                    {
                        ITypeConverter converter = TypeConverterFactory.GetConvertType(propInfo.PropertyType);

                        if (converter != null)
                            propInfo.SetValue(item, converter.Convert(value), null);
                        else
                            propInfo.SetValue(item, value, null);
                    }
                    else
                    {
                        if (propInfo.PropertyType.IsValueType)
                            propInfo.SetValue(item, Activator.CreateInstance(propInfo.PropertyType), null);
                        else
                            propInfo.SetValue(item, string.Empty, null);
                    }
                }

                return item;
            }
        }

        private dynamic BindingDataRecordToModelDynamic(IDataRecord Record)
        {
            // handle with "dynamic".
            dynamic item = new ExpandoObject();
            var d = ((IDictionary<string, object>)item);

            for (int i = 0; i < Record.FieldCount; i++)
            {
                ((IDictionary<string, object>)item).Add(
                    Record.GetName(i).Trim(), (Record.IsDBNull(i)) ? null : Record.GetValue(i));
            }

            return item;
        }

        public void Open()
        {
            this._manageManually = true;
            this._sqlExecutionProvider.Open();
        }

        public void Close()
        {
            this._manageManually = false;
            this._sqlExecutionProvider.Close();
        }
    }
}
