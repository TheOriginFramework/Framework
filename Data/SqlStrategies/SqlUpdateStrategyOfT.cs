using TOF.Framework.Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data.SqlStrategies
{
    public class SqlUpdateStrategy<TModel> : SqlStrategyBase<TModel>,
        ISqlQueryIncrementalStrategy,
        ISqlQueryDecrementalStrategy
        where TModel : class, new()
    {
        public SqlUpdateStrategy() : base()
        {
        }

        public SqlUpdateStrategy(IModelStrategy ModelStrategy) : base(ModelStrategy)
        {
        }

        public override string RenderQuery()
        {
            if (base.ModelStrategy == null)
                throw new ArgumentNullException("ERROR_MODEL_STRATEGY_NOT_FOUND");
 
            var propBindings = base.ModelStrategy.GetModelPropertyBindings();

            if (!propBindings.Where(c => c.IsKey()).Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            var updateQueryBuilder = new StringBuilder();
            var updateColumnBuilder = new StringBuilder();
            var updateClausesBuilder = new StringBuilder();

            updateQueryBuilder.Append("UPDATE ");
            updateQueryBuilder.Append(base.ModelStrategy.GetTableName());
            updateQueryBuilder.Append(" SET ");
            
            foreach (var column in base.Columns)
            {
                if (updateColumnBuilder.Length > 0)
                    updateColumnBuilder.Append(", ");

                var propNameQuery = propBindings.Where(c => c.GetParameterName() == column);

                if (propNameQuery.Any())
                {
                    updateColumnBuilder.Append(column);
                    updateColumnBuilder.Append(" = ");
                    updateColumnBuilder.Append("@" + propNameQuery.First().GetParameterName());

                    if (propNameQuery.First().IsKey())
                    {
                        updateClausesBuilder.Append(column);
                        updateClausesBuilder.Append(" = ");
                        updateClausesBuilder.Append("@" + propNameQuery.First().GetParameterName());
                    }

                    continue;
                }

                var propPropertyInfoQuery = propBindings.Where(c => c.GetPropertyInfo().Name == column);

                if (propPropertyInfoQuery.Any())
                {
                    updateColumnBuilder.Append(column);
                    updateColumnBuilder.Append(" = ");
                    updateColumnBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);

                    if (propPropertyInfoQuery.First().IsKey())
                    {
                        updateClausesBuilder.Append(column);
                        updateClausesBuilder.Append(" = ");
                        updateClausesBuilder.Append("@" + propPropertyInfoQuery.First().GetPropertyInfo().Name);
                    }

                    continue;
                }

                updateColumnBuilder.Append(column);
                updateColumnBuilder.Append(" = ");
                updateColumnBuilder.Append("@" + column);
            }

            updateQueryBuilder.Append(updateColumnBuilder.ToString());
            updateQueryBuilder.Append(" WHERE ");            
            updateQueryBuilder.Append(updateClausesBuilder);

            return updateQueryBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> RenderParameters()
        {
            var propBindings = this.ModelStrategy.GetModelPropertyBindings();

            if (!propBindings.Where(c => c.IsKey()).Any())
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

        public string RenderIncreaseQuery()
        {
            return this.RenderIncreaseDecreaseUpdateQueryInternal(true);
        }

        public IEnumerable<IDbDataParameter> RenderIncreaseParameters()
        {
            var keyBindings = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.IsKey());

            if (!keyBindings.Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            foreach (var propBinding in keyBindings)
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

        public string RenderDecreaseQuery()
        {
            return this.RenderIncreaseDecreaseUpdateQueryInternal(false);
        }

        public IEnumerable<IDbDataParameter> RenderDecreaseParameters()
        {
            var keyBindings = this.ModelStrategy.GetModelPropertyBindings().Where(c => c.IsKey());

            if (!keyBindings.Any())
                throw new InvalidOperationException("ERROR_MODEL_KEY_NOT_FOUND");

            foreach (var propBinding in keyBindings)
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

        public bool CanSupportIncrementalOperation()
        {
            if (this.ModelStrategy == null)
                return false;

            return this.ModelStrategy.GetModelPropertyBindings().Where(c => c.IsIncremental()).Any();
        }

        public bool CanSupportDecrementalOperation()
        {
            if (this.ModelStrategy == null)
                return false;

            return this.ModelStrategy.GetModelPropertyBindings().Where(c => c.IsIncremental()).Any();
        }

        private string RenderIncreaseDecreaseUpdateQueryInternal(bool IsIncrease)
        {
            if (base.ModelStrategy == null)
                throw new ModelStrategyException("ERROR_MODEL_STRATEGY_NOT_FOUND");

            if (!this.CanSupportIncrementalOperation())
                throw new ModelStrategyException(
                    "ERROR_MODEL_STRATEGY_NOT_SUPPORT_INCREMENETAL_OPERATION");

            var propBindings = base.ModelStrategy.GetModelPropertyBindings();

            if (!propBindings.Where(c => c.IsKey()).Any())
                throw new ModelStrategyException("ERROR_MODEL_KEY_NOT_FOUND");

            var updateQueryBuilder = new StringBuilder();
            var updateColumnBuilder = new StringBuilder();
            var updateClausesBuilder = new StringBuilder();

            updateQueryBuilder.Append("UPDATE ");
            updateQueryBuilder.Append(base.ModelStrategy.GetTableName());
            updateQueryBuilder.Append(" SET ");

            var incrementalColumns = propBindings.Where(c => c.IsIncremental());

            if (!incrementalColumns.Any())
                return null;

            // create incremental fields.
            foreach (var f in incrementalColumns)
            {
                if (updateColumnBuilder.Length > 0)
                    updateColumnBuilder.Append(", ");

                updateColumnBuilder.Append(f.GetParameterName());
                updateColumnBuilder.Append(" = ");
                updateColumnBuilder.Append(f.GetParameterName());

                if (IsIncrease)
                    updateColumnBuilder.Append(" + 1 ");
                else
                    updateColumnBuilder.Append(" - 1 ");
            }

            // create key fields.
            var keyFields = propBindings.Where(c => c.IsKey());

            foreach (var f in keyFields)
            {
                updateClausesBuilder.Append(f.GetParameterName());
                updateClausesBuilder.Append(" = ");
                updateClausesBuilder.Append("@" + f.GetParameterName());
            }

            updateQueryBuilder.Append(updateColumnBuilder.ToString());
            updateQueryBuilder.Append(" WHERE ");
            updateQueryBuilder.Append(updateClausesBuilder);

            return updateQueryBuilder.ToString();
        }
    }
}
