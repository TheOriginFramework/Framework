using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ITable<TModel> where TModel: class
    {
        #region Data Query Configuration
        ITable<TModel> Name(string Name);
        ITable<TModel> Alias(string Alias);
        ITable<TModel> TransactionRequiredForMultipleOperation();
        ITable<TModel> TransactionNotRequiredForMultipleOperation();
        ITable<TModel> ConfigureDbConnection(string ConnectionString);   
        #endregion

        ITable<TModel> Where(Expression<Func<TModel, object>> WhereConditionSpecifier);
        ITable<TModel> WhereAnd(Expression<Func<TModel, object>> WhereConditionSpecifier);
        ITable<TModel> WhereOr(Expression<Func<TModel, object>> WhereConditionSpecifier);
        ITable<TModel> WhereNotAnd(Expression<Func<TModel, object>> WhereConditionSpecifier);
        ITable<TModel> WhereNotOr(Expression<Func<TModel, object>> WhereConditionSpecifier);
        ITable<TModel> OrderBy(Expression<Func<TModel, object>> OrderBySpecifier);
        ITable<TModel> OrderByDesc(Expression<Func<TModel, object>> OrderBySpecifier);
        
        #region Configuration Getters
        string GetName();
        string GetAlias();
        string GetLastQueryStatement();
        string GetConnectionString();
        IModelStrategy GetTableModelStrategy();
        #endregion

        #region Create Operation
        void Create(TModel Model);
        void Create(IEnumerable<TModel> Models);
        void Create(TModel Model, IModelStrategy ModelBindingStrategy);
        void Create(IEnumerable<TModel> Models, IModelStrategy ModelBindingStrategy);
        #endregion

        #region Data Update Operation
        void Update(TModel Model);
        void UpdateIncrease(TModel Model);
        void UpdateDecrease(TModel Model);
        void Update(IEnumerable<TModel> Models);
        void Update(TModel Model, IModelStrategy ModelBindingStrategy);
        void UpdateIncrease(TModel Model, IModelStrategy ModelBindingStrategy);
        void UpdateDecrease(TModel Model, IModelStrategy ModelBindingStrategy);
        void Update(IEnumerable<TModel> Models, IModelStrategy ModelBindingStrategy);
        #endregion

        #region Data Delete Operation
        void Delete(TModel Model);
        void Delete(IEnumerable<TModel> Models);
        void Delete(TModel Model, IModelStrategy ModelBindingStrategy);
        void Delete(IEnumerable<TModel> Models, IModelStrategy ModelBindingStrategy);
        #endregion

        #region Data Query Operations
        IEnumerable<TModel> Select();
        IEnumerable<dynamic> Select(Expression<Func<TModel, object>> Selector);
        #endregion

        #region Data Query Paging Operation
        IQueryPagingContext<TModel> SelectPaging(int PageSize);
        IQueryPagingContext SelectPaging(int PageSize, Expression<Func<TModel, object>> Selector);
        #endregion

        #region Aggregations
        int Count();
        IEnumerable<dynamic> Count(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        long CountAsLong();
        IEnumerable<dynamic> CountAsLong(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        bool Any();
        decimal Average(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Average(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Sum(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Sum(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Max(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Max(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Min(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Min(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Var(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Var(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal VarForPopulation(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> VarForPopulation(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal StdDev(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> StdDev(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal StdDevForPopulation(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> StdDevForPopulation(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        #endregion
    }
}
