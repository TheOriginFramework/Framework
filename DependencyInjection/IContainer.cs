using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public interface IContainer
    {
        #region Generic Resolvers.

        TInterface Resolve<TInterface>();
        TInterface Resolve<TInterface>(params object[] ConstructorParams);
        TInterface Resolve<TInterface, TConcreteClass>() where TConcreteClass : TInterface;
        TInterface Resolve<TInterface, TConcreteClass>(params object[] ConstructorParams) where TConcreteClass : TInterface;
        
        #endregion

        #region Non-Generic Resolvers.

        object Resolve(Type InterfaceType, params object[] ConstructorParams);
        object Resolve(Type InterfaceType, Type ConcreteType, params object[] ConstructorParams);

        #endregion

        #region Lifetimes Management.
        ILifetimeManager BeginLifetimeManagement();
        #endregion
    }
}
