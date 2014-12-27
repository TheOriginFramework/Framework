using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultDbProcedureStrategy : IDbProcedureStrategy
    {
        private Type _modelType = null;
        private string _procedureName = null;
        private IDictionary<string, IParameterBindingInfo> _procedurePropertyBindings = null;

        public DefaultDbProcedureStrategy(
            Type ModelType, string ProcedureName, IDictionary<string, IParameterBindingInfo> ProcedurePropertyBindings = null)
        {
            this._modelType = ModelType;
            this._procedureName = ProcedureName;

            if (ProcedurePropertyBindings == null)
                this._procedurePropertyBindings = new Dictionary<string, IParameterBindingInfo>();
            else
                this._procedurePropertyBindings = new Dictionary<string, IParameterBindingInfo>(ProcedurePropertyBindings);

            this.Initialize();
        }

        private void Initialize()
        {
        }

        public IParameterBindingInfo DefineParameter<T>(string ParameterName, Expression<Func<T, object>> PropertySpecifier)
        {
            if (typeof(T) != this._modelType)
                throw new ArgumentException("ERROR_MODEL_TYPE_MISMATCH");

            SqlQueryExpressionNode expressionNode = new SqlQueryGetMemberNameExpressionNode();
            string propertyName = expressionNode.Parse(PropertySpecifier.Body);

            var prop = typeof(T).GetProperty(propertyName);

            if (prop == null)
                throw new ArgumentException("ERROR_PROPERTY_NOT_FOUND");

            IParameterBindingInfo propertyBindingInfo = new DefaultParameterBindingInfo(prop);
            this.AddOrReplaceParameterBindingInfo(ParameterName, propertyBindingInfo);
            return propertyBindingInfo;
        }

        public string GetProcedureName()
        {
            return this._procedureName;
        }

        public IEnumerable<IDbDataParameter> RenderParameters()
        {
            foreach (var propBinding in this._procedurePropertyBindings)
            {
                SqlParameter param = new SqlParameter();

                param.ParameterName = propBinding.Key;

                if (propBinding.Value.GetMapDbType() != null)
                    param.DbType = propBinding.Value.GetMapDbType().Value;
                if (propBinding.Value.GetLength() != null)
                    param.Size = propBinding.Value.GetLength().Value;

                yield return param;
            }
        }

        public IDictionary<string, IParameterBindingInfo> GetParameterBindings()
        {
            return this._procedurePropertyBindings;
        }

        private void AddOrReplaceParameterBindingInfo(string ParameterName, IParameterBindingInfo ParameterBindingInfo)
        {
            if (this._procedurePropertyBindings.ContainsKey(ParameterName))
                this._procedurePropertyBindings[ParameterName] = ParameterBindingInfo;
            else
                this._procedurePropertyBindings.Add(ParameterName, ParameterBindingInfo);
        }
    }
}
