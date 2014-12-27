using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public abstract class SqlStrategyBase<TModel> : ISqlQueryStrategy
        where TModel: class, new()
    {
        protected IEnumerable<string> Columns { get; set; }
        protected IModelStrategy ModelStrategy { get; set; }

        public SqlStrategyBase()
        {
            this.Parse();
        }

        public SqlStrategyBase(IModelStrategy ModelStrategy)
        {
            this.ModelStrategy = ModelStrategy;
            this.Parse();
        }

        protected virtual void Parse()
        {
            List<string> columns = new List<string>();
            
            if (this.ModelStrategy != null)
            {
                if (this.ModelStrategy.GetModelPropertyBindings().Any())
                {
                    var modelBindings = this.ModelStrategy.GetModelPropertyBindings();
                    var propQuery = typeof(TModel).GetProperties();

                    foreach (var prop in propQuery)
                    {
                        var modelBindingQuery = modelBindings.Where(
                            c => c.GetPropertyInfo().Name == prop.Name);

                        if (modelBindingQuery.Any())
                        {
                            if (string.IsNullOrEmpty(modelBindingQuery.First().GetParameterName()))
                                columns.Add(prop.Name);
                            else
                                columns.Add(modelBindingQuery.First().GetParameterName());
                        }
                        else
                            columns.Add(prop.Name);
                    }
                }
                else
                {
                    var propQuery = typeof(TModel).GetProperties();

                    foreach (var prop in propQuery)
                        columns.Add(prop.Name);
                }
            }
            else
            {
                var propQuery = typeof(TModel).GetProperties();

                foreach (var prop in propQuery)
                    columns.Add(prop.Name);
            }

            this.Columns = columns;
        }

        public abstract string RenderQuery();

        public virtual IEnumerable<IDbDataParameter> RenderParameters()
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();

            if (this.ModelStrategy != null)
            {

            }
            else
            {
                foreach (var column in this.Columns)
                {
                    var prop = typeof(TModel).GetProperty(column);
                    var bindingInfo = new DefaultPropertyBindingInfo(prop);
                    ISqlParameterParser parser = (new SqlParameterParser()).ForProperty(bindingInfo);
                    parameters.Add(parser.GetParameter());
                }
            }

            return parameters;
        }
    }
}
