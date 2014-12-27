using TOF.Framework.Data;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using TOF.Framework.Contracts.Exceptions;
using TOF.Framework.Utils.TypeConverters;
using TOF.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace TOF.Framework.Data
{
    public class DefaultTable : ITable
    {
        private Type _modelType = null;
        private IModelStrategy _defaultStrategy = null;
        private string _connectionString = null;
        private string _tableName = null;
        private string _tableAlias = null;
        private string _tableLastQueryStatement = null;
        private bool _transactionRequiredInMultipleOperation = true;
        private List<IQueryWhereExpression> _queryWhereExpressions = new List<IQueryWhereExpression>();
        private List<IQueryOrderByExpression> _queryOrderByExpressions = new List<IQueryOrderByExpression>();

        public DefaultTable(Type ModelType)
        {
            this._modelType = ModelType;
            this.Name(this._modelType.Name);
        }

        public DefaultTable(Type ModelType, IModelStrategy DefaultStrategy)
            : this(ModelType)
        {
            this._defaultStrategy = DefaultStrategy;
        }

        public DefaultTable(Type ModelType, string TableName)
        {
            this.Name(TableName);
        }

        public DefaultTable(Type ModelType, string TableName, IModelStrategy DefaultStrategy)
            : this(ModelType, TableName)
        {
            this._defaultStrategy = DefaultStrategy;
        }

        public ITable Name(string Name)
        {
            this._tableName = Name;

            if (string.IsNullOrEmpty(this._tableAlias))
                this._tableAlias = this._tableName.ToLower();

            return this;
        }

        public ITable Alias(string Alias)
        {
            this._tableAlias = Alias;
            return this;
        }

        public ITable TransactionRequiredForMultipleOperation()
        {
            this._transactionRequiredInMultipleOperation = true;
            return this;
        }

        public ITable TransactionNotRequiredForMultipleOperation()
        {
            this._transactionRequiredInMultipleOperation = false;
            return this;
        }

        public ITable ConfigureDbConnection(string ConnectionString)
        {
            this._connectionString = ConnectionString;
            return this;
        }

        public string GetName()
        {
            return this._tableName;
        }

        public string GetConnectionString()
        {
            return this._connectionString;
        }

        public string GetAlias()
        {
            return this._tableAlias;
        }
        
        public Type GetDefinedModelType()
        {
            return this._modelType;
        }
        
        public IModelStrategy GetTableModelStrategy()
        {
            if (this._defaultStrategy != null)
                return this._defaultStrategy;
            else
                return new DefaultModelStrategy(this._modelType, this._tableName, this._tableAlias, null);
        }

        public ITable Where<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new()
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).Begin(WhereConditionSpecifier));

            return this;
        }

        public ITable WhereAnd<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new()
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).And(WhereConditionSpecifier));

            return this;
        }

        public ITable WhereOr<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new()
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).Or(WhereConditionSpecifier));

            return this;
        }

        public ITable WhereNotAnd<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new()
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).NotAnd(WhereConditionSpecifier));

            return this;
        }

        public ITable WhereNotOr<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new()
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).NotOr(WhereConditionSpecifier));

            return this;
        }

        public ITable OrderBy<TModel>(Expression<Func<TModel, object>> OrderBySpecifier) where TModel: class, new()
        {
            this._queryOrderByExpressions.Add(
                new DefaultQueryOrderByExpression<TModel>()
                .OrderBy(OrderBySpecifier));

            return this;
        }

        public ITable OrderByDesc<TModel>(Expression<Func<TModel, object>> OrderBySpecifier) where TModel : class, new()
        {
            this._queryOrderByExpressions.Add(
                new DefaultQueryOrderByExpression<TModel>()
                .OrderByDesc(OrderBySpecifier));

            return this;
        }

        public void Create<TModel>(TModel Model)
        {
            if (this._defaultStrategy != null)
            {
                this.Create(Model, this._defaultStrategy);
                return;
            }

            IModelStrategy modelStrategy = new DefaultModelStrategy(this._modelType, this._tableName, this._tableAlias, null);
            this.InternalCreate(Model, modelStrategy);
        }

        public void Create<TModel>(IEnumerable<TModel> Models)
        {
            this.InternalCreate(Models, this.GetTableModelStrategy());
        }

        public void Create<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetInsertProcedure() != null)
            {
                IDbProcedureInvoker invoker = new DefaultDbProcedureInvoker(ModelStrategy.GetInsertProcedure());
                invoker.Invoke(Model);
                invoker = null;
            }
            else
            {
                this.InternalCreate(Model, ModelStrategy);
            }
        }

        public void Create<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetInsertProcedure() != null)
            {
                IDbProcedureInvoker invoker = new DefaultDbProcedureInvoker(ModelStrategy.GetInsertProcedure());
                invoker.Invoke(Models);
                invoker = null;
            }
            else
            {
                this.InternalCreate(Models, ModelStrategy);
            }
        }

        private void InternalCreate<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            // build INSERT statement to do query.
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlInsertStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = this.BindingModelPropertyToParameter(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();

            try
            {
                executionProvider.Open();

                if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        private void InternalCreate<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            DbOperationExceptionCollector exceptionCollector = new DbOperationExceptionCollector();
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlInsertStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider =
                Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this._connectionString });
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();

            if (this._transactionRequiredInMultipleOperation)
            {
                if (transactionProvider == null)
                    throw new DbEnvironmentException("ERROR_TRANSACTION_PROVIDER_IS_NOT_FOUND");

                transactionProvider.BeginTransaction();
            }

            foreach (var model in Models)
            {
                // binding parameters.
                parameters = this.BindingModelPropertyToParameter(model, ModelStrategy.GetModelPropertyBindings(), parameters);

                // execute operation.
                try
                {
                    if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                    {
                        exceptionCollector.Add(
                            new DbOperationException(
                            "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters));
                    }
                }
                catch (Exception exception)
                {
                    exceptionCollector.Add(
                        new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters));

                    if (this._transactionRequiredInMultipleOperation)
                    {
                        transactionProvider.Rollback();
                        break;
                    }
                }
            }

            if (this._transactionRequiredInMultipleOperation)
            {
                transactionProvider.Commit();
            }

            executionProvider.Close();

            if (exceptionCollector.HasException())
                throw new DbMultipleOperationsException(exceptionCollector.GetDbExceptions());
        }

        public void Update<TModel>(TModel Model)
        {
            this.InternalUpdate(Model, this.GetTableModelStrategy());

        }

        public void Update<TModel>(IEnumerable<TModel> Models)
        {
            if (this._defaultStrategy != null)
            {
                this.Update(Models, this._defaultStrategy);
                return;
            }

            IModelStrategy modelStrategy = new DefaultModelStrategy(this._modelType, this._tableName, this._tableAlias, null);
            this.InternalUpdate(Models, modelStrategy);

        }

        public void Update<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetUpdateProcedure() != null)
            {
                IDbProcedureInvoker invoker = new DefaultDbProcedureInvoker(ModelStrategy.GetUpdateProcedure());
                invoker.Invoke(Model);
                invoker = null;
            }
            else
            {
                this.InternalUpdate(Model, ModelStrategy);
            }
        }

        public void Update<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetUpdateProcedure() != null)
            {
                IDbProcedureInvoker invoker = new DefaultDbProcedureInvoker(ModelStrategy.GetUpdateProcedure());
                invoker.Invoke(Models);
                invoker = null;
            }
            else
            {
                this.InternalUpdate(Models, ModelStrategy);
            }
        }

        private void InternalUpdate<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = this.BindingModelPropertyToParameter(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();

            try
            {
                executionProvider.Open();

                if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        private void InternalUpdate<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            DbOperationExceptionCollector exceptionCollector = new DbOperationExceptionCollector();
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();

            if (this._transactionRequiredInMultipleOperation)
            {
                if (transactionProvider == null)
                    throw new DbEnvironmentException("ERROR_TRANSACTION_PROVIDER_IS_NOT_FOUND");

                transactionProvider.BeginTransaction();
            }

            foreach (var model in Models)
            {
                // binding parameters.
                parameters = this.BindingModelPropertyToParameter(model, ModelStrategy.GetModelPropertyBindings(), parameters);

                // execute operation.
                try
                {

                    if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                    {
                        exceptionCollector.Add(
                            new DbOperationException("ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters));
                    }
                }
                catch (Exception exception)
                {
                    exceptionCollector.Add(
                        new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters));

                    if (this._transactionRequiredInMultipleOperation)
                    {
                        transactionProvider.Rollback();
                        break;
                    }
                }
            }

            if (this._transactionRequiredInMultipleOperation)
            {
                transactionProvider.Commit();
            }

            executionProvider.Close();

            if (exceptionCollector.HasException())
                throw new DbMultipleOperationsException(exceptionCollector.GetDbExceptions());
        }

        public void UpdateIncrease<TModel>(TModel Model)
        {
            this.InternalUpdateIncrease<TModel>(Model, this.GetTableModelStrategy());
        }

        public void UpdateIncrease<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            this.InternalUpdateIncrease(Model, ModelStrategy);
        }

        private void InternalUpdateIncrease<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryIncrementalStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderIncreaseQuery();
            var parameters = queryStrategy.RenderIncreaseParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = this.BindingModelPropertyToParameter(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();

            try
            {
                executionProvider.Open();

                if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        public void UpdateDecrease<TModel>(TModel Model)
        {
            this.InternalUpdateDecrease<TModel>(Model, this.GetTableModelStrategy());
        }

        public void UpdateDecrease<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            this.InternalUpdateDecrease(Model, ModelStrategy);
        }

        private void InternalUpdateDecrease<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryDecrementalStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderDecreaseQuery();
            var parameters = queryStrategy.RenderDecreaseParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = this.BindingModelPropertyToParameter(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();

            try
            {
                executionProvider.Open();

                if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        public void Delete<TModel>(TModel Model)
        {
            this.InternalDelete(Model, this.GetTableModelStrategy());
        }

        public void Delete<TModel>(IEnumerable<TModel> Models)
        {
            this.InternalDelete(Models, this.GetTableModelStrategy());
        }

        public void Delete<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetDeleteProcedure() != null)
            {
                IDbProcedureInvoker invoker = new DefaultDbProcedureInvoker(ModelStrategy.GetDeleteProcedure());
                invoker.Invoke(Model);
                invoker = null;
            }
            else
            {
                this.InternalDelete(Model, ModelStrategy);
            }
        }

        public void Delete<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetDeleteProcedure() != null)
            {
                IDbProcedureInvoker invoker = new DefaultDbProcedureInvoker(ModelStrategy.GetDeleteProcedure());
                invoker.Invoke(Models);
                invoker = null;
            }
            else
            {
                this.InternalDelete(Models, ModelStrategy);
            }
        }

        private void InternalDelete<TModel>(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlDeleteStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = this.BindingModelPropertyToParameter(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();

            try
            {
                executionProvider.Open();

                if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                {
                    throw new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters);
                }
            }
            catch (Exception exception)
            {
                throw new DbOperationException(
                    "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters);
            }
            finally
            {
                executionProvider.Close();
            }
        }

        private void InternalDelete<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            DbOperationExceptionCollector exceptionCollector = new DbOperationExceptionCollector();
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlDeleteStrategy(this._modelType, ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();

            if (this._transactionRequiredInMultipleOperation)
            {
                if (transactionProvider == null)
                    throw new DbEnvironmentException("ERROR_TRANSACTION_PROVIDER_IS_NOT_FOUND");

                transactionProvider.BeginTransaction();
            }

            foreach (var model in Models)
            {
                // binding parameters.
                parameters = this.BindingModelPropertyToParameter(model, ModelStrategy.GetModelPropertyBindings(), parameters);

                // execute operation.
                try
                {

                    if (executionProvider.Execute(this._tableLastQueryStatement, parameters) == 0)
                    {
                        exceptionCollector.Add(
                            new DbOperationException(
                            "ERROR_SQL_EXECUTION_FAILED", this._tableLastQueryStatement, parameters));
                    }
                }
                catch (Exception exception)
                {
                    exceptionCollector.Add(
                        new DbOperationException(
                        "ERROR_SQL_EXECUTION_FAILED", exception, this._tableLastQueryStatement, parameters));

                    if (this._transactionRequiredInMultipleOperation)
                    {
                        transactionProvider.Rollback();
                        break;
                    }
                }
            }

            if (this._transactionRequiredInMultipleOperation)
            {
                transactionProvider.Commit();
            }

            executionProvider.Close();

            if (exceptionCollector.HasException())
                throw new DbMultipleOperationsException(exceptionCollector.GetDbExceptions());
        }

        public IEnumerable<TModel> Select<TModel>()
        {
            return this.Select<TModel>(this.GetTableModelStrategy());
        }

        public IEnumerable<dynamic> Select<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.Select(this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<TModel> Select<TModel>(IModelStrategy ModelStrategy)
        {
            ISqlQueryStrategy queryStrategy =
                (new SqlStrategies.SqlSelectStrategy(this._modelType, ModelStrategy))
                .PrepareFilters<TModel>(this._queryWhereExpressions)
                .PrepareOrderByColumns(this._queryOrderByExpressions);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();
            var propertyBindings = ModelStrategy.GetModelPropertyBindings();

            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();
            IDataReader reader = executionProvider.ExecuteGetReader(this._tableLastQueryStatement, null);

            List<TModel> items = new List<TModel>();
            var records = this.GetDataRecords(reader);

            foreach (var record in records)
                items.Add(this.BindingDataRecordToModel<TModel>(record, ModelStrategy.GetModelPropertyBindings()));

            reader.Close();
            executionProvider.Close();

            return items;
        }

        public IEnumerable<dynamic> Select<TModel>(IModelStrategy ModelStrategy, Expression<Func<TModel, object>> Selector)
        {
            ISqlQueryStrategy queryStrategy =
                (new SqlStrategies.SqlSelectStrategy(this._modelType, ModelStrategy))
                .PrepareColumnSelector<TModel>(Selector)
                .PrepareFilters<TModel>(this._queryWhereExpressions)
                .PrepareOrderByColumns(this._queryOrderByExpressions);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = this.GetDbExecutionProvider();
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            SqlQueryNewTypeRenderColumnNode selectorParser = new Expressions.SqlQueryNewTypeRenderColumnNode(ModelStrategy);
            IDictionary<string, string> columns = selectorParser.Parse(Selector.Body);

            executionProvider.Open();
            IDataReader reader = executionProvider.ExecuteGetReader(this._tableLastQueryStatement, null);

            List<dynamic> items = new List<dynamic>();
            var records = this.GetDataRecords(reader);

            foreach (var record in records)
            {
                dynamic item = new ExpandoObject();
                IDictionary<string, object> itemProps = item as IDictionary<string, object>;

                foreach (var column in columns)
                {
                    try
                    {
                        if (reader.GetOrdinal(column.Value.Trim()) >= 0)
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), reader.GetValue(reader.GetOrdinal(column.Value.Trim())));
                        else
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), DBNull.Value);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        ((IDictionary<string, object>)item).Add(column.Key.Trim(), DBNull.Value);
                    }
                }

                items.Add(item);
            }

            reader.Close();
            executionProvider.Close();

            return items;
        }

        public int Count<TModel>()
        {
            return this.ExecuteAggregationComputes<TModel, int>(SqlSelectAggregateMode.Count, this.GetTableModelStrategy());
        }

        public IEnumerable<dynamic> Count<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.Count, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public long CountAsLong<TModel>()
        {
            return this.ExecuteAggregationComputes<TModel, long>(SqlSelectAggregateMode.CountBig, this.GetTableModelStrategy());
        }

        public IEnumerable<dynamic> CountAsLong<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.CountBig, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public bool Any<TModel>()
        {
            return this.Count<TModel>() > 0;
        }
        
        public decimal Average<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, int>(SqlSelectAggregateMode.Average, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Average<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.Average, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Sum<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, int>(SqlSelectAggregateMode.Sum, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Sum<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.Sum, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Max<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, int>(SqlSelectAggregateMode.Max, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Max<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.Max, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Min<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, int>(SqlSelectAggregateMode.Min, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Min<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.Min, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Var<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, decimal>(SqlSelectAggregateMode.Var, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Var<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.Var, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal VarForPopulation<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, decimal>(SqlSelectAggregateMode.VarP, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> VarForPopulation<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.VarP, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal StdDev<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, decimal>(SqlSelectAggregateMode.StdDev, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> StdDev<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.StdDev, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal StdDevForPopulation<TModel>(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<TModel, decimal>(SqlSelectAggregateMode.StdDevP, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> StdDevForPopulation<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes<TModel>(SqlSelectAggregateMode.StdDevP, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }
        
        public string GetLastQueryStatement()
        {
            return this._tableLastQueryStatement;
        }
        
        private IEnumerable<IDbDataParameter> BindingModelPropertyToParameter<TModel>(
            TModel Model, IEnumerable<IPropertyBindingInfo> PropertyBindingInfoItems, IEnumerable<IDbDataParameter> Parameters)
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>(Parameters);

            foreach (var Parameter in parameters)
            {
                var propBindingQuery = PropertyBindingInfoItems
                    .Where(c => c.GetParameterName() == (Parameter.ParameterName.Substring(1)));

                if (propBindingQuery.Any())
                {
                    var PropertyBindingInfo = propBindingQuery.First();
                    PropertyInfo modelProperty = PropertyBindingInfo.GetPropertyInfo();
                    object value = modelProperty.GetValue(Model, null);

                    if (value != null)
                    {
                        ITypeConverter converter = TypeConverterFactory.GetConvertType(
                            (PropertyBindingInfo.GetMapDbType() == DbType.Guid)
                            ? typeof(Guid)
                            : modelProperty.PropertyType);

                        if (converter != null)
                        {
                            if (converter is EnumConverter)
                                Parameter.Value =
                                    (converter as EnumConverter).Convert(modelProperty.PropertyType, value);
                            else
                                Parameter.Value = converter.Convert(value);
                        }
                        else
                        {
                            if (value.GetType().IsValueType)
                                Parameter.Value = value;
                            else
                                Parameter.Value = value.ToString();
                        }
                    }
                    else
                    {
                        if (PropertyBindingInfo.IsAllowNull())
                            Parameter.Value = DBNull.Value;
                        else
                        {
                            if (PropertyBindingInfo.GetPropertyInfo().PropertyType.IsValueType)
                            {
                                Parameter.Value = Activator.CreateInstance(
                                    PropertyBindingInfo.GetPropertyInfo().PropertyType);
                            }
                            else
                                Parameter.Value = string.Empty;
                        }
                    }
                }
            }

            return parameters;
        }

        private TModel BindingDataRecordToModel<TModel>(IDataRecord Record, IEnumerable<IPropertyBindingInfo> PropertyBindingInfoItems)
        {
            // assign result to property.
            TModel item = (TModel)Activator.CreateInstance(typeof(TModel));

            for (int i = 0; i < Record.FieldCount; i++)
            {
                var propQuery = PropertyBindingInfoItems.Where(c => c.GetParameterName() == Record.GetName(i));

                if (propQuery.Any())
                {
                    var propInfo = propQuery.First().GetPropertyInfo();
                    object value = Record.GetValue(i);

                    if (value != null && value != DBNull.Value)
                    {
                        ITypeConverter converter = TypeConverterFactory.GetConvertType(propInfo.PropertyType);

                        if (converter != null)
                            propInfo.SetValue(item, converter.Convert(value), null);
                        else
                            propInfo.SetValue(item, value, null);
                    }
                    else
                    {
                        if (propQuery.First().IsAllowNull())
                            propInfo.SetValue(item, null, null);
                        else
                        {
                            if (propInfo.PropertyType.IsValueType)
                                propInfo.SetValue(item, Activator.CreateInstance(propInfo.PropertyType), null);
                            else
                                propInfo.SetValue(item, string.Empty, null);
                        }
                    }
                }
            }

            return item;
        }

        private TValue ExecuteAggregationComputes<TModel, TValue>(
            SqlSelectAggregateMode Mode,
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector = null) where TValue : struct
        {
            var queryStrategy = new SqlStrategies.SqlSelectAggregateStrategy(Mode, typeof(TModel), ModelStrategy);

            queryStrategy
                .PrepareColumnSelector<TModel>(Selector)
                .PrepareFilters<TModel>(this._queryWhereExpressions);

            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            var executionProvider = this.GetDbExecutionProvider();
            var transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();
            IDataReader reader = executionProvider.ExecuteGetReader(this._tableLastQueryStatement, null);

            if (!reader.Read())
                throw new InvalidOperationException("ERROR_SQL_SUM_EXECUTION_FAILED");

            decimal result = Convert.ToDecimal(reader.GetValue(0));

            reader.Close();
            executionProvider.Close();

            return (TValue)Convert.ChangeType(result, typeof(TValue));
        }

        private IEnumerable<dynamic> ExecuteAggregationComputes<TModel>(
            SqlSelectAggregateMode Mode,
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector,
            Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            var queryStrategy = new SqlStrategies.SqlSelectAggregateStrategy(Mode, typeof(TModel), ModelStrategy);

            queryStrategy
                .PrepareColumnSelector<TModel>(Selector)
                .PrepareFilters<TModel>(this._queryWhereExpressions)
                .PrepareGroupBySelector<TModel>(GroupBySelectors);

            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            var executionProvider = this.GetDbExecutionProvider();
            var transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            var selectorColumns = new Dictionary<string, string>();
            var selectorParser = new Expressions.SqlQueryNewTypeRenderColumnNode(ModelStrategy);

            foreach (var selector in selectorParser.Parse(Selector.Body))
                selectorColumns.Add(selector.Key, selector.Value);

            if (GroupBySelectors != null && GroupBySelectors.Length > 0)
            {
                foreach (var GroupBySelector in GroupBySelectors)
                {
                    var groupByColumn = selectorParser.Parse(GroupBySelector.Body);

                    foreach (var selector in groupByColumn)
                        selectorColumns.Add(selector.Key, selector.Value);
                }
            }

            executionProvider.Open();
            IDataReader reader = executionProvider.ExecuteGetReader(this._tableLastQueryStatement, null);

            List<dynamic> items = new List<dynamic>();

            while (reader.Read())
            {
                dynamic item = new ExpandoObject();

                foreach (var column in selectorColumns)
                {
                    try
                    {
                        if (reader.GetOrdinal(column.Value.Trim()) >= 0)
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), reader.GetValue(reader.GetOrdinal(column.Value.Trim())));
                        else
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), DBNull.Value);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        ((IDictionary<string, object>)item).Add(column.Key.Trim(), DBNull.Value);
                    }
                }

                items.Add(item);
            }

            reader.Close();
            executionProvider.Close();

            return items;
        }

        private ISqlExecutionProvider GetDbExecutionProvider()
        {
            return Application.GetServices(DataTypeRegistrationContainer.Key)
                .Resolve<ISqlExecutionProvider>(new object[] { this._connectionString });
        }

        private IEnumerable<IDataRecord> GetDataRecords(IDataReader reader)
        {
            while (reader.Read())
                yield return reader;
        }
    }
}
