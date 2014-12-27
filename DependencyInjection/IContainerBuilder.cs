using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public interface IContainerBuilder
    {
        #region Simple Type Registration.
        Registration Register<TInterface>(string RegistrationName = null);
        Registration Register(Type TypeToRegister, string RegistrationName = null);
        #endregion

        #region Types in assembly Registration.
        void RegisterTypesInAssembly(Assembly AssemblyToRegister, bool LoadFromStartupInitializer = true, string RegistrationName = null);
        #endregion

        #region Find Type Registration.
        IEnumerable<Registration> Find<T>(string RegistrationName = null);
        IEnumerable<Registration> Find(Type TypeToFind, string RegistrationName = null);
        IEnumerable<Registration> FindByConcrete<T>(string RegistrationName = null);
        IEnumerable<Registration> FindByConcrete(Type TypeToFind, string RegistrationName = null);
        #endregion

        #region Setup Container Type.
        void UseContainer<TContainer>() where TContainer : IContainer;
        void UseContainer(Type ContainerType);
        #endregion

        #region Generate Container
        IContainer Build(string RegistrationName = null);
        #endregion
    }
}
