using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultQuery : IQuery
    {
        public QueryTypes QueryType { get; private set; }
        public IList<IQueryParameter> Parameters { get; private set; }

        public IQuery AsDirectQuery()
        {
            this.QueryType = QueryTypes.DirectQuery;
            return this;
        }

        public IQuery AsTableOrViewQuery()
        {
            this.QueryType = QueryTypes.TableOrViewQuery;
            return this;
        }

        public IQuery AsProcedureQuery()
        {
            this.QueryType = QueryTypes.ProcedureQuery;
            return this;
        }

        public IQueryParameter DefineQueryParameter<TQueryParameter>(string Name)
            where TQueryParameter: IQueryParameter
        {
            var parameter = ParameterFactory.DefineParameter<TQueryParameter>();
            parameter.Name(Name);

            if (this.Parameters == null)
                this.Parameters = new List<IQueryParameter>();

            this.Parameters.Add(parameter);
            return parameter;
        }
    }
}
