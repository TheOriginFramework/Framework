using TOF.Framework.Data;
using TOF.Framework.Data.Expressions;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.SqlStrategies;
using TOF.Framework.Contracts.Exceptions;
using TOF.Framework.Utils.TypeConverters;
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
    public class DefaultTable<TModel> : ITable<TModel>
        where TModel : class, new()
    {
        private IModelStrategy _defaultStrategy = null;
        private string _connectionString = null;
        private string _tableName = null;
        private string _tableAlias = null;
        private string _tableLastQueryStatement = null;
        private List<IQueryOrderByExpression> _queryOrderByExpressions = new List<IQueryOrderByExpression>();
        private List<IQueryWhereExpression> _queryWhereExpressions = new List<IQueryWhereExpression>();
        private bool _transactionRequiredInMultipleOperation = true;

        public DefaultTable()
        {
            this.Name(typeof(TModel).Name);
        }

        public DefaultTable(IModelStrategy DefaultStrategy)
            : this()
        {
            this._defaultStrategy = DefaultStrategy;
        }

        public DefaultTable(string TableName)
        {
            this.Name(TableName);
        }

        public DefaultTable(string TableName, IModelStrategy DefaultStrategy)
            : this(TableName)
        {
            this._defaultStrategy = DefaultStrategy;
        }

        public ITable<TModel> Name(string Name)
        {
            this._tableName = Name;

            if (string.IsNullOrEmpty(this._tableAlias))
                this._tableAlias = this._tableName.ToLower();

            return this;
        }

        public ITable<TModel> Alias(string Alias)
        {
            this._tableAlias = Alias;
            return this;
        }

        public ITable<TModel> TransactionRequiredForMultipleOperation()
        {
            this._transactionRequiredInMultipleOperation = true;
            return this;
        }

        public ITable<TModel> TransactionNotRequiredForMultipleOperation()
        {
            this._transactionRequiredInMultipleOperation = false;
            return this;
        }

        public ITable<TModel> ConfigureDbConnection(string ConnectionString)
        {
            this._connectionString = ConnectionString;
            return this;
        }

        public ITable<TModel> Where(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).Begin(WhereConditionSpecifier));

            return this;
        }

        public ITable<TModel> WhereAnd(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).And(WhereConditionSpecifier));

            return this;
        }

        public ITable<TModel> WhereOr(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).Or(WhereConditionSpecifier));

            return this;
        }

        public ITable<TModel> WhereNotAnd(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).NotAnd(WhereConditionSpecifier));

            return this;
        }

        public ITable<TModel> WhereNotOr(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._queryWhereExpressions.Add(
                new DefaultQueryWhereExpression<TModel>(this.GetTableModelStrategy()).NotOr(WhereConditionSpecifier));

            return this;
        }

        public ITable<TModel> OrderBy(Expression<Func<TModel, object>> OrderBySpecifier)
        {
            this._queryOrderByExpressions.Add(
                new DefaultQueryOrderByExpression<TModel>()
                .OrderBy(OrderBySpecifier));

            return this;
        }

        public ITable<TModel> OrderByDesc(Expression<Func<TModel, object>> OrderBySpecifier)
        {
            this._queryOrderByExpressions.Add(
                new DefaultQueryOrderByExpression<TModel>()
                .OrderByDesc(OrderBySpecifier));

            return this;
        }

        public string GetName()
        {
            return this._tableName;
        }

        public string GetAlias()
        {
            return this._tableAlias;
        }

        public string GetConnectionString()
        {
            return this._connectionString;
        }

        public IModelStrategy GetTableModelStrategy()
        {
            if (this._defaultStrategy != null)
                return this._defaultStrategy;
            else
            {
                IModelStrategy modelStrategy = new DefaultModelStrategy<TModel>(this._tableName, this._tableAlias, null);
                return modelStrategy;
            }
        }

        public void Create(TModel Model)
        {
            this.InternalCreate(Model, this.GetTableModelStrategy());
        }

        public void Create(IEnumerable<TModel> Models)
        {
            this.InternalCreate(Models, this.GetTableModelStrategy());
        }

        public void Create(TModel Model, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetInsertProcedure() != null)
            {
                IDbProcedureInvoker<TModel> invoker = new DefaultDbProcedureInvoker<TModel>(ModelStrategy.GetInsertProcedure());
                invoker.Invoke(Model);
                invoker = null;
            }
            else
            {
                this.InternalCreate(Model, ModelStrategy);
            }
        }

        public void Create(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetInsertProcedure() != null)
            {
                IDbProcedureInvoker<TModel> invoker = new DefaultDbProcedureInvoker<TModel>(ModelStrategy.GetInsertProcedure());
                invoker.Invoke(Models);
                invoker = null;
            }
            else
            {
                this.InternalCreate(Models, ModelStrategy);
            }
        }

        private void InternalCreate(TModel Model, IModelStrategy ModelStrategy)
        {
            // build INSERT statement to do query.
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlInsertStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = Utils.BindingModelPropertyToParameter<TModel>(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);

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

        private void InternalCreate(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            DbOperationExceptionCollector exceptionCollector = new DbOperationExceptionCollector();
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlInsertStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
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
                parameters = Utils.BindingModelPropertyToParameter<TModel>(model, ModelStrategy.GetModelPropertyBindings(), parameters);

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

        public void Update(TModel Model)
        {
            if (this._defaultStrategy != null)
            {
                this.Update(Model, this._defaultStrategy);
                return;
            }

            IModelStrategy modelStrategy = new DefaultModelStrategy<TModel>(this._tableName, this._tableAlias, null);
            this.InternalUpdate(Model, modelStrategy);

        }

        public void Update(IEnumerable<TModel> Models)
        {
            this.InternalUpdate(Models, this.GetTableModelStrategy());

        }

        public void Update(TModel Model, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetUpdateProcedure() != null)
            {
                IDbProcedureInvoker<TModel> invoker = new DefaultDbProcedureInvoker<TModel>(ModelStrategy.GetUpdateProcedure());
                invoker.Invoke(Model);
                invoker = null;
            }
            else
            {
                this.InternalUpdate(Model, ModelStrategy);
            }
        }

        public void Update(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetUpdateProcedure() != null)
            {
                IDbProcedureInvoker<TModel> invoker = new DefaultDbProcedureInvoker<TModel>(ModelStrategy.GetUpdateProcedure());
                invoker.Invoke(Models);
                invoker = null;
            }
            else
            {
                this.InternalUpdate(Models, ModelStrategy);
            }
        }

        private void InternalUpdate(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = Utils.BindingModelPropertyToParameter<TModel>(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);

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

        private void InternalUpdate(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            DbOperationExceptionCollector exceptionCollector = new DbOperationExceptionCollector();
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
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
                parameters = Utils.BindingModelPropertyToParameter<TModel>(model, ModelStrategy.GetModelPropertyBindings(), parameters);

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

        public void UpdateIncrease(TModel Model)
        {
            this.InternalUpdateIncrease(Model, this.GetTableModelStrategy());
        }

        public void UpdateIncrease(TModel Model, IModelStrategy ModelStrategy)
        {
            this.InternalUpdateIncrease(Model, ModelStrategy);
        }

        private void InternalUpdateIncrease(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryIncrementalStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderIncreaseQuery();
            var parameters = queryStrategy.RenderIncreaseParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = Utils.BindingModelPropertyToParameter<TModel>(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);

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

        public void UpdateDecrease(TModel Model)
        {
            this.InternalUpdateDecrease(Model, this.GetTableModelStrategy());
        }

        public void UpdateDecrease(TModel Model, IModelStrategy ModelStrategy)
        {
            this.InternalUpdateDecrease(Model, ModelStrategy);
        }

        private void InternalUpdateDecrease(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryDecrementalStrategy queryStrategy = new SqlStrategies.SqlUpdateStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderDecreaseQuery();
            var parameters = queryStrategy.RenderDecreaseParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = Utils.BindingModelPropertyToParameter<TModel>(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);

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

        public void Delete(TModel Model)
        {
            this.InternalDelete(Model, this.GetTableModelStrategy());
        }

        public void Delete(IEnumerable<TModel> Models)
        {
            this.InternalDelete(Models, this.GetTableModelStrategy());
        }

        public void Delete(TModel Model, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetDeleteProcedure() != null)
            {
                IDbProcedureInvoker<TModel> invoker = new DefaultDbProcedureInvoker<TModel>(ModelStrategy.GetDeleteProcedure());
                invoker.Invoke(Model);
                invoker = null;
            }
            else
            {
                this.InternalDelete(Model, ModelStrategy);
            }
        }

        public void Delete(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            if (ModelStrategy.GetDeleteProcedure() != null)
            {
                IDbProcedureInvoker<TModel> invoker = new DefaultDbProcedureInvoker<TModel>(ModelStrategy.GetDeleteProcedure());
                invoker.Invoke(Models);
                invoker = null;
            }
            else
            {
                this.InternalDelete(Models, ModelStrategy);
            }
        }

        private void InternalDelete(TModel Model, IModelStrategy ModelStrategy)
        {
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlDeleteStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            parameters = Utils.BindingModelPropertyToParameter<TModel>(Model, ModelStrategy.GetModelPropertyBindings(), parameters);

            // execute statement to database.
            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);

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

        private void InternalDelete(IEnumerable<TModel> Models, IModelStrategy ModelStrategy)
        {
            DbOperationExceptionCollector exceptionCollector = new DbOperationExceptionCollector();
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlDeleteStrategy<TModel>(ModelStrategy);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
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
                parameters = Utils.BindingModelPropertyToParameter<TModel>(model, ModelStrategy.GetModelPropertyBindings(), parameters);

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

        public IEnumerable<TModel> Select()
        {
            return this.Select(this.GetTableModelStrategy());
        }

        public IEnumerable<dynamic> Select(Expression<Func<TModel, object>> Selector)
        {
            return this.Select(this.GetTableModelStrategy(), Selector);
        }
        
        internal IEnumerable<TModel> Select(IModelStrategy ModelStrategy)
        {
            ISqlQueryStrategy queryStrategy = new SqlStrategies.SqlSelectStrategy<TModel>(
                ModelStrategy,
                FilterExpressions: this._queryWhereExpressions,
                OrderByExpressions: this._queryOrderByExpressions);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();
            var propertyBindings = ModelStrategy.GetModelPropertyBindings();

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            IDataReader reader = null;
            List<TModel> items = new List<TModel>();

            executionProvider.Open();

            try
            {
                var records = executionProvider.ExecuteQuery(this._tableLastQueryStatement, null);
                items = new List<TModel>();

                foreach (var record in records)
                    items.Add(Utils.BindingDataRecordToModel<TModel>(record, ModelStrategy.GetModelPropertyBindings()));                
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                reader.Close();
                executionProvider.Close();        
                
                // clean where and order by conditions.
                this._queryWhereExpressions.Clear();
                this._queryOrderByExpressions.Clear();
            }

            return items;
        }

        internal IEnumerable<dynamic> Select(IModelStrategy ModelStrategy, Expression<Func<TModel, object>> Selector)
        {
            ISqlQueryStrategy queryStrategy =
                new SqlStrategies.SqlSelectStrategy<TModel>(
                    ModelStrategy, 
                    Selector,
                    FilterExpressions: this._queryWhereExpressions,
                    OrderByExpressions: this._queryOrderByExpressions);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            SqlQueryNewTypeRenderColumnNode selectorParser = new Expressions.SqlQueryNewTypeRenderColumnNode(ModelStrategy);
            IDictionary<string, string> columns = selectorParser.Parse(Selector.Body);
            List<dynamic> items = new List<dynamic>();

            executionProvider.Open();

            try
            {
                var records = executionProvider.ExecuteQuery(this._tableLastQueryStatement, null);
                items = new List<dynamic>();

                foreach (var record in records)
                {
                    dynamic item = new ExpandoObject();
                    IDictionary<string, object> itemProps = item as IDictionary<string, object>;

                    foreach (var column in columns)
                    {
                        try
                        {
                            if (record.GetOrdinal(column.Value.Trim()) >= 0)
                                ((IDictionary<string, object>)item).Add(column.Key.Trim(), record.GetValue(record.GetOrdinal(column.Value.Trim())));
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
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                executionProvider.Close();

                // clean where and order by conditions.
                this._queryWhereExpressions.Clear();
                this._queryOrderByExpressions.Clear();
            }

            return items;
        }

        public IQueryPagingContext<TModel> SelectPaging(int PageSize)
        {
            return new DefaultSelectPagingContext<TModel>(this, PageSize, 
                FilterExpressions: this._queryWhereExpressions,
                OrderByExpressions: this._queryOrderByExpressions);
        }

        public IQueryPagingContext SelectPaging(int PageSize, Expression<Func<TModel, object>> Selector)
        {
            return new DefaultSelectPagingContextForDynamic<TModel>(this, PageSize,
                Selector: Selector,
                FilterExpressions: this._queryWhereExpressions,
                OrderByExpressions: this._queryOrderByExpressions);
        }

        public string GetLastQueryStatement()
        {
            return this._tableLastQueryStatement;
        }

        public int Count()
        {
            return this.ExecuteAggregationComputes<int>(SqlSelectAggregateMode.Count, this.GetTableModelStrategy());
        }

        public IEnumerable<dynamic> Count(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.Count, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public long CountAsLong()
        {
            return this.ExecuteAggregationComputes<long>(SqlSelectAggregateMode.CountBig, this.GetTableModelStrategy());
        }

        public IEnumerable<dynamic> CountAsLong(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.CountBig, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public bool Any()
        {
            return this.Count() > 0;
        }

        public decimal Average(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.Average, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Average(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.Average, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Sum(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.Sum, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Sum(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.Sum, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Max(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.Max, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Max(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.Max, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Min(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.Min, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Min(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.Min, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal Var(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.Var, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> Var(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.Var, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal VarForPopulation(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.VarP, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> VarForPopulation(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.VarP, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal StdDev(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.StdDev, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> StdDev(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.StdDev, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        public decimal StdDevForPopulation(Expression<Func<TModel, object>> Selector)
        {
            return this.ExecuteAggregationComputes<decimal>(SqlSelectAggregateMode.StdDevP, this.GetTableModelStrategy(), Selector);
        }

        public IEnumerable<dynamic> StdDevForPopulation(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            return this.ExecuteAggregationComputes(SqlSelectAggregateMode.StdDevP, this.GetTableModelStrategy(), Selector, GroupBySelectors);
        }

        private TValue ExecuteAggregationComputes<TValue>(
            SqlSelectAggregateMode Mode,
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector = null) where TValue: struct
        {
            var queryStrategy = new SqlStrategies.SqlSelectAggregateStrategy<TModel>(Mode, ModelStrategy, Selector, this._queryWhereExpressions, null);
            
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            var executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
            var transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();
            var records = executionProvider.ExecuteQuery(this._tableLastQueryStatement, null);

            if (!records.Any())
                throw new InvalidOperationException("ERROR_SQL_SUM_EXECUTION_FAILED");

            decimal result = Convert.ToDecimal(records.First().GetValue(0));

            executionProvider.Close();

            // clean where and order by conditions.
            this._queryWhereExpressions.Clear();
            this._queryOrderByExpressions.Clear();

            return (TValue)Convert.ChangeType(result, typeof(TValue));
        }

        private IEnumerable<dynamic> ExecuteAggregationComputes(
            SqlSelectAggregateMode Mode,
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector,
            Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            var queryStrategy =
                new SqlStrategies.SqlSelectAggregateStrategy<TModel>(Mode, ModelStrategy, Selector, this._queryWhereExpressions, GroupBySelectors);
            this._tableLastQueryStatement = queryStrategy.RenderQuery();
            var parameters = queryStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();

            var executionProvider = Utils.GetDbExecutionProvider(this._connectionString);
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
            var records = executionProvider.ExecuteQuery(this._tableLastQueryStatement, null);

            List<dynamic> items = new List<dynamic>();

            foreach (var record in records)
            {
                dynamic item = new ExpandoObject();

                foreach (var column in selectorColumns)
                {
                    try
                    {
                        if (record.GetOrdinal(column.Value.Trim()) >= 0)
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), record.GetValue(record.GetOrdinal(column.Value.Trim())));
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

            executionProvider.Close();

            // clean where and order by conditions.
            this._queryWhereExpressions.Clear();
            this._queryOrderByExpressions.Clear();

            return items;
        }
    }
}
