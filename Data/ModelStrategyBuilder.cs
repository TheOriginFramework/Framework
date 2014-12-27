using TOF.Framework.Data.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class ModelStrategyBuilder
    {
        private IDictionary<Type, object> _modelStrategies = null;
        private IDictionary<string, IDbProcedureStrategy> _procedureStrategies = null;
        private IDictionary<string, Type> _bindingModelTypes = null;

        public ModelStrategyBuilder()
        {
            this._modelStrategies = new Dictionary<Type, object>();
            this._procedureStrategies = new Dictionary<string, IDbProcedureStrategy>();
            this._bindingModelTypes = new Dictionary<string, Type>();
        }

        public IModelStrategy GetTableStrategy(string TableName)
        {
            var strategyQuery = this._modelStrategies.Where(c => 
                (c.Value as IModelStrategy).GetTableName() == TableName);

            if (strategyQuery.Any())
                return strategyQuery.First().Value as IModelStrategy;
            else
                return null;
        }

        public IModelStrategy GetTableStrategy(Type ModelType)
        {
            if (this._modelStrategies.ContainsKey(ModelType))
                return this._modelStrategies[ModelType] as IModelStrategy;
            else
                return null;
        }

        public IModelStrategy GetTableStrategy<TModel>()
        {
            return this.GetTableStrategy(typeof(TModel));
        }

        public void BindingModelTypeToProperty(string PropertyName, Type ModelType)
        {
            this._bindingModelTypes.Add(PropertyName, ModelType);
        }

        public Type GetModelTypeFromProperty(string PropertyName)
        {
            return this._bindingModelTypes[PropertyName];
        }

        public IDbProcedureStrategy GetProcedureStrategy(string ProcedureName)
        {
            if (this._procedureStrategies.ContainsKey(ProcedureName))
                return this._procedureStrategies[ProcedureName];
            else
                return null;
        }

        public IModelStrategy Table(Type ModelType)
        {
            string tableName = ModelType.Name;
            TableAttribute[] attrTableNames = (TableAttribute[])
                ModelType.GetCustomAttributes(typeof(TableAttribute), true);

            if (attrTableNames != null && attrTableNames.Length > 0)
                tableName = attrTableNames[0].TableName;

            var modelStrategy = new DefaultModelStrategy(ModelType, tableName);

            if (this._modelStrategies.ContainsKey(ModelType))
            {
                return this._modelStrategies[ModelType] as IModelStrategy;
            }
            else
            {
                this._modelStrategies.Add(ModelType, modelStrategy);
                return modelStrategy;
            }
        }

        public IModelStrategy Table(Type ModelType, string TableName) 
        {
            var modelStrategy = new DefaultModelStrategy(ModelType, TableName);

            if (this._modelStrategies.ContainsKey(ModelType))
            {
                return this._modelStrategies[ModelType] as IModelStrategy;
            }
            else
            {
                this._modelStrategies.Add(ModelType, modelStrategy);
                return modelStrategy;
            }
        }

        public IModelStrategy Table(Type ModelType, string TableName, string TableAlias) 
        {
            var modelStrategy = new DefaultModelStrategy(ModelType, TableName, TableAlias);

            if (this._modelStrategies.ContainsKey(ModelType))
            {
                return this._modelStrategies[ModelType] as IModelStrategy;
            }
            else
            {
                this._modelStrategies.Add(ModelType, modelStrategy);
                return modelStrategy;
            }
        }

        public IModelStrategy<TModel> Table<TModel>() where TModel : class, new()
        {
            string tableName = typeof(TModel).Name;
            TableAttribute[] attrTableNames = (TableAttribute[])
                typeof(TModel).GetCustomAttributes(typeof(TableAttribute), true);

            if (attrTableNames != null && attrTableNames.Length > 0)
                tableName = attrTableNames[0].TableName;

            var modelStrategy = new DefaultModelStrategy<TModel>(tableName);

            if (this._modelStrategies.ContainsKey(typeof(TModel)))
            {
                return this._modelStrategies[typeof(TModel)] as IModelStrategy<TModel>;
            }
            else
            {
                this._modelStrategies.Add(typeof(TModel), modelStrategy);
                return modelStrategy;
            }
        }

        public IModelStrategy<TModel> Table<TModel>(string TableName) where TModel : class, new()
        {
            var modelStrategy = new DefaultModelStrategy<TModel>(TableName);

            if (this._modelStrategies.ContainsKey(typeof(TModel)))
            {
                return this._modelStrategies[typeof(TModel)] as IModelStrategy<TModel>;
            }
            else
            {
                this._modelStrategies.Add(typeof(TModel), modelStrategy);
                return modelStrategy;
            }
        }

        public IModelStrategy<TModel> Table<TModel>(string TableName, string TableAlias) where TModel : class, new()
        {
            var modelStrategy = new DefaultModelStrategy<TModel>(TableName, TableAlias);

            if (this._modelStrategies.ContainsKey(typeof(TModel)))
            {
                return this._modelStrategies[typeof(TModel)] as IModelStrategy<TModel>;
            }
            else
            {
                this._modelStrategies.Add(typeof(TModel), modelStrategy);
                return modelStrategy;
            }
        }

        public IDbProcedureStrategy Procedure(Type ModelType, string ProcedureName)
        {
            var procedureStrategy = new DefaultDbProcedureStrategy(ModelType, ProcedureName);
            this._procedureStrategies.Add(ProcedureName, procedureStrategy);
            return procedureStrategy;
        }

        public IDbProcedureStrategy<TModel> Procedure<TModel>(string ProcedureName) where TModel : class, new()
        {
            var procedureStrategy = new DefaultDbProcedureStrategy<TModel>(ProcedureName);
            this._procedureStrategies.Add(ProcedureName, procedureStrategy);
            return procedureStrategy;
        }
    }
}
