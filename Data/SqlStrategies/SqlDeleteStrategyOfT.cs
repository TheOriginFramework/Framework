using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data.SqlStrategies
{
    public class SqlDeleteStrategy<TModel> : SqlStrategyBase<TModel>
        where TModel : class, new()
    {
        public SqlDeleteStrategy() : base()
        {
        }

        public SqlDeleteStrategy(IModelStrategy ModelStrategy) : base(ModelStrategy)
        {
        }

        public override string RenderQuery()
        {
            if (base.ModelStrategy == null)
                throw new ArgumentNullException("ERROR_MODEL_STRATEGY_NOT_FOUND");

            var propBindings = base.ModelStrategy.GetModelPropertyBindings();

            if (!propBindings.Where(c => c.IsKey()).Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            var deleteQueryBuilder = new StringBuilder();
            var deleteClausesBuilder = new StringBuilder();

            deleteQueryBuilder.Append("DELETE FROM ");
            deleteQueryBuilder.Append(base.ModelStrategy.GetTableName());

            foreach (var column in base.Columns)
            {
                var propNameQuery = propBindings.Where(c => c.GetParameterName() == column);

                if (propNameQuery.Any())
                {
                    if (propNameQuery.First().IsKey())
                    {
                        deleteClausesBuilder.Append(column);
                        deleteClausesBuilder.Append(" = ");
                        deleteClausesBuilder.Append("@" + propNameQuery.First().GetParameterName());
                    }

                    continue;
                }

                var propPropertyInfoQuery = propBindings.Where(c => c.GetPropertyInfo().Name == column);

                if (propPropertyInfoQuery.Any())
                {
                    if (propPropertyInfoQuery.First().IsKey())
                    {
                        deleteClausesBuilder.Append(column);
                        deleteClausesBuilder.Append(" = ");
                        deleteClausesBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);
                    }

                    continue;
                }
            }

            deleteQueryBuilder.Append(" WHERE ");
            deleteQueryBuilder.Append(deleteClausesBuilder);

            return deleteQueryBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> RenderParameters()
        {
            var propBindings = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.IsKey());

            if (!propBindings.Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            foreach (var propBinding in propBindings)
            {
                SqlParameter param = new SqlParameter();

                param.ParameterName = "@" + propBinding.GetParameterName();

                if (propBinding.GetMapDbType() != null)
                    param.DbType = propBinding.GetMapDbType().Value;
                if (propBinding.GetLength() != null)
                    param.Size = propBinding.GetLength().Value;

                yield return param;
            }
        }
    }
}
