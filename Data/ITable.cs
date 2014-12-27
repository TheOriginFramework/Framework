using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ITable
    {
        ITable Name(string Name);
        ITable Alias(string Alias);
        ITable TransactionRequiredForMultipleOperation();
        ITable TransactionNotRequiredForMultipleOperation();
        ITable ConfigureDbConnection(string ConnectionString);
        string GetName();
        string GetAlias();
        string GetLastQueryStatement();
        Type GetDefinedModelType();
        IModelStrategy GetTableModelStrategy();
        ITable Where<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new();
        ITable WhereAnd<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new();
        ITable WhereOr<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new();
        ITable WhereNotAnd<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new();
        ITable WhereNotOr<TModel>(Expression<Func<TModel, object>> WhereConditionSpecifier) where TModel : class, new();
        ITable OrderBy<TModel>(Expression<Func<TModel, object>> OrderBySpecifier) where TModel : class, new();
        ITable OrderByDesc<TModel>(Expression<Func<TModel, object>> OrderBySpecifier) where TModel : class, new();
        void Create<TModel>(TModel Model);
        void Create<TModel>(IEnumerable<TModel> Models);
        void Create<TModel>(TModel Model, IModelStrategy ModelBindingStrategy);
        void Create<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelBindingStrategy);
        void Update<TModel>(TModel Model);
        void UpdateIncrease<TModel>(TModel Model);
        void UpdateDecrease<TModel>(TModel Model);
        void Update<TModel>(IEnumerable<TModel> Models);
        void Update<TModel>(TModel Model, IModelStrategy ModelBindingStrategy);
        void UpdateIncrease<TModel>(TModel Model, IModelStrategy ModelBindingStrategy);
        void UpdateDecrease<TModel>(TModel Model, IModelStrategy ModelBindingStrategy);
        void Update<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelBindingStrategy);
        void Delete<TModel>(TModel Model);
        void Delete<TModel>(IEnumerable<TModel> Models);
        void Delete<TModel>(TModel Model, IModelStrategy ModelBindingStrategy);
        void Delete<TModel>(IEnumerable<TModel> Models, IModelStrategy ModelBindingStrategy);
        IEnumerable<TModel> Select<TModel>();
        IEnumerable<dynamic> Select<TModel>(Expression<Func<TModel, object>> Selector);
        int Count<TModel>();
        IEnumerable<dynamic> Count<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        long CountAsLong<TModel>();
        IEnumerable<dynamic> CountAsLong<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        bool Any<TModel>();
        decimal Average<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Average<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Sum<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Sum<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Min<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Min<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Max<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Max<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal Var<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> Var<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal VarForPopulation<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> VarForPopulation<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal StdDev<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> StdDev<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
        decimal StdDevForPopulation<TModel>(Expression<Func<TModel, object>> Selector);
        IEnumerable<dynamic> StdDevForPopulation<TModel>(Expression<Func<TModel, object>> Selector, params Expression<Func<TModel, object>>[] GroupBySelectors);
    }
}
