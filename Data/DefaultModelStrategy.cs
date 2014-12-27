using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TOF.Framework.Data;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using System.Reflection;

namespace TOF.Framework.Data
{
    public class DefaultModelStrategy : IModelStrategy
    {
        private Type _modelType = null;
        private string _tableName = null;
        private string _tableAlias = null;
        private List<IPropertyBindingInfo> _modelPropertyBindings = null;

        // C/U/D operation via stored procedure strategy.
        private IDbProcedureStrategy _insertProcStrategy = null;
        private IDbProcedureStrategy _updateProcStrategy = null;
        private IDbProcedureStrategy _deleteProcStrategy = null;

        public DefaultModelStrategy(
            Type ModelType, string TableName, string TableAlias = null,
            IEnumerable<IPropertyBindingInfo> ModelPropertyBindings = null)
        {
            this._modelType = ModelType;
            this._tableName = TableName;

            if (!string.IsNullOrEmpty(TableAlias))
                this._tableAlias = TableAlias;
            else
                this._tableAlias = _tableName.ToLower();

            if (ModelPropertyBindings == null)
                this._modelPropertyBindings = new List<IPropertyBindingInfo>();
            else
                this._modelPropertyBindings = new List<IPropertyBindingInfo>(ModelPropertyBindings);

            this.Initialize();
        }

        private void Initialize()
        {
            // create default property binding information.
            var propQuery = this._modelType.GetProperties();

            if (!propQuery.Any())
                return;

            foreach (var prop in propQuery)
            {
                var propBindingInfo = new DefaultPropertyBindingInfo(prop);
                this.AddOrReplacePropertyBindingInfo(prop.Name, propBindingInfo);
            }
        }

        public string GetTableName()
        {
            return this._tableName;
        }

        public string GetTableAlias()
        {
            return this._tableAlias;
        }

        public Type GetModelType()
        {
            return this._modelType;
        }

        public void ChangeTableName(string TableName)
        {
            this._tableName = TableName;
        }

        public void ChangeTableAlias(string TableAlias)
        {
            this._tableAlias = TableAlias;
        }

        public IEnumerable<IPropertyBindingInfo> GetModelPropertyBindings()
        {
            return this._modelPropertyBindings;
        }

        public IPropertyBindingInfo DefineProperty<T>(Expression<Func<T, object>> PropertySpecifier)
        {
            if (typeof(T) != this._modelType)
                throw new ArgumentException("ERROR_MODEL_TYPE_MISMATCH");

            SqlQueryExpressionNode expressionNode = new SqlQueryGetMemberNameExpressionNode();
            expressionNode.ModelStrategy = this;
            string propertyName = expressionNode.Parse(PropertySpecifier.Body);

            var prop = typeof(T).GetProperty(propertyName);

            if (prop == null)
                throw new ArgumentException("ERROR_PROPERTY_NOT_FOUND");

            IPropertyBindingInfo propertyBindingInfo = new DefaultPropertyBindingInfo(prop);
            this.AddOrReplacePropertyBindingInfo(prop.Name, propertyBindingInfo);
            return propertyBindingInfo;
        }

        public IPropertyBindingInfo DefinePropertyExact(PropertyInfo Property)
        {
            IPropertyBindingInfo propertyBindingInfo = new DefaultPropertyBindingInfo(Property);
            this.AddOrReplacePropertyBindingInfo(Property.Name, propertyBindingInfo);
            return propertyBindingInfo;
        }

        public IDbProcedureStrategy DefineInsertProcedure(string ProcedureName)
        {
            this._insertProcStrategy = new DefaultDbProcedureStrategy(this._modelType, ProcedureName);
            return this._insertProcStrategy;
        }

        public IDbProcedureStrategy DefineUpdateProcedure(string ProcedureName)
        {
            this._updateProcStrategy = new DefaultDbProcedureStrategy(this._modelType, ProcedureName);
            return this._updateProcStrategy;
        }
        
        public IDbProcedureStrategy GetInsertProcedure()
        {
            return this._insertProcStrategy;
        }

        public IDbProcedureStrategy GetUpdateProcedure()
        {
            return this._updateProcStrategy;
        }

        public IDbProcedureStrategy GetDeleteProcedure()
        {
            return this._deleteProcStrategy;
        }

        public IDbProcedureStrategy DefineDeleteProcedure(string ProcedureName)
        {
            this._deleteProcStrategy = new DefaultDbProcedureStrategy(this._modelType, ProcedureName);
            return this._deleteProcStrategy;
        }

        private void AddOrReplacePropertyBindingInfo(string PropertyName, IPropertyBindingInfo PropertyBindingInfo)
        {
            if (this._modelPropertyBindings.Where(c => c.GetPropertyInfo().Name == PropertyName).Any())
            {
                this._modelPropertyBindings.Remove(
                    this._modelPropertyBindings.Where(c => c.GetPropertyInfo().Name == PropertyName).First());
            }

            this._modelPropertyBindings.Add(PropertyBindingInfo);
        }
    }
}
