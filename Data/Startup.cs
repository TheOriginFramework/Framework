using TOF.Framework.DependencyInjection;
using TOF.Framework.Data.SqlExecutionProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Startup(typeof(TOF.Framework.Data.Startup))]

namespace TOF.Framework.Data
{
    public class Startup : IStartup
    {
        public void Initialize(IContainerBuilder builder)
        {
            builder.Register<DefaultQuery>(DataTypeRegistrationContainer.Key).As<ISqlQuery>();
            builder.Register<DefaultTable>(DataTypeRegistrationContainer.Key).As<ITable>();
            builder.Register<DefaultDbProcedureInvoker>(DataTypeRegistrationContainer.Key).As<IDbProcedureInvoker>();
            builder.Register<DefaultModelStrategy>(DataTypeRegistrationContainer.Key).As<IModelStrategy>();
            builder.Register<DefaultParameterBindingInfo>(DataTypeRegistrationContainer.Key).As<IParameterBindingInfo>();
            builder.Register<DefaultPropertyBindingInfo>(DataTypeRegistrationContainer.Key).As<IPropertyBindingInfo>();
            builder.Register<SqlExecutionProvider>(DataTypeRegistrationContainer.Key).As<ISqlExecutionProvider>();

            builder.Register(typeof(DefaultTable<>)).As(typeof(ITable<>));
        }
    }
}
