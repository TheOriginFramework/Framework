using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultSqlQuery : ISqlQuery
    {
        private string _sqlQuery = null;
        private IDictionary<string, object> _parameterModel = null;

        public DefaultSqlQuery(string SqlQuery, IDictionary<string, object> ParameterModel)
        {
            this._sqlQuery = SqlQuery;
            this._parameterModel = ParameterModel;
        }

        public string GetSqlStatement()
        {
            return this._sqlQuery;
        }

        public IEnumerable<IDbDataParameter> GetParameters()
        {
            foreach (var p in this._parameterModel)
                yield return new SqlParameter(p.Key, p.Value);
        }

        public void UpdateParameter(string Name, object Value)
        {
            if (!this._parameterModel.ContainsKey(Name))
                this._parameterModel.Add(Name, Value);
            else
                this._parameterModel[Name] = Value;
        }
    }
}
