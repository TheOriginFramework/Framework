using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public class ContainerBuilder : IContainerBuilder, IContainerConfigurationBuilder
    {
        private Dictionary<string, Dictionary<Type, Registration>> _containerRegistrations = null;
        private const string DefaultContainerKey = "DEFAULT";
        private Type _containerType = typeof(Container);

        public ContainerBuilder()
        {
            this._containerRegistrations = new Dictionary<string, Dictionary<Type, Registration>>();
            this._containerRegistrations.Add(DefaultContainerKey, new Dictionary<Type, Registration>());
        }

        public Registration Register<TRegister>(string RegistrationName = null)
        {
            return this.Register(typeof(TRegister), RegistrationName);
        }

        public Registration Register(Type TypeToRegister, string RegistrationName = null)
        {
            var registration = new Registration(TypeToRegister);

            if (!string.IsNullOrEmpty(RegistrationName))
            {
                if (!this._containerRegistrations.ContainsKey(RegistrationName))
                    this._containerRegistrations.Add(RegistrationName, new Dictionary<Type, Registration>());

                if (this._containerRegistrations[RegistrationName].ContainsKey(TypeToRegister))
                    this._containerRegistrations[RegistrationName][TypeToRegister] = registration;
                else
                    this._containerRegistrations[RegistrationName].Add(TypeToRegister, registration);
            }
            else
            {
                if (this._containerRegistrations[DefaultContainerKey].ContainsKey(TypeToRegister))
                    this._containerRegistrations[DefaultContainerKey][TypeToRegister] = registration;
                else
                    this._containerRegistrations[DefaultContainerKey].Add(TypeToRegister, registration);
            }

            return registration;
        }

        public void RegisterTypesInAssembly(Assembly AssemblyToRegister, bool LoadFromStartupInitializer = true, string RegistrationName = null)
        {
            if (LoadFromStartupInitializer)
                this.InitializeFromStartup(AssemblyToRegister);
            else
                this.InitializeFromAssembly(AssemblyToRegister, RegistrationName);
        }

        public void UseContainer<TContainer>() where TContainer : IContainer
        {
            this.UseContainer(typeof(TContainer));
        }

        public void UseContainer(Type ContainerType)
        {
            if (ContainerType == null)
                throw new ArgumentNullException("ERROR_TYPE_NOT_FOUND");
            if (!ContainerType.GetInterfaces().Where(i => i.Name == "IContainer").Any())
                throw new ArgumentException("ERROR_ILLEGAL_CONTAINER_TYPE");

            this._containerType = ContainerType;
        }

        public IEnumerable<Registration> Find<T>(string RegistrationName = null)
        {
            return this.Find(typeof(T), RegistrationName);
        }

        public IEnumerable<Registration> Find(Type TypeToFind, string RegistrationName = null)
        {
            string containerToFind =
                (string.IsNullOrEmpty(RegistrationName)) ? DefaultContainerKey : RegistrationName;

            if (TypeToFind.IsInterface)
            {
                var itemQuery =
                    this._containerRegistrations[containerToFind].Where(
                    c => c.Value.MapInterfaceType == TypeToFind);

                foreach (var item in itemQuery)
                    yield return item.Value;
            }
            else
            {
                var itemQuery =
                    this._containerRegistrations[containerToFind].Where(
                    c => c.Value.MapConcreteType == TypeToFind);

                foreach (var item in itemQuery)
                    yield return item.Value;
            }
        }

        public IEnumerable<Registration> FindByConcrete<T>(string RegistrationName = null)
        {
            return this.FindByConcrete(typeof(T), RegistrationName);
        }

        public IEnumerable<Registration> FindByConcrete(Type TypeToFind, string RegistrationName = null)
        {
            string containerToFind =
                (string.IsNullOrEmpty(RegistrationName)) ? DefaultContainerKey : RegistrationName;

            if (TypeToFind.IsInterface)
                throw new TypeIncompatibilityException();
            else
            {
                var itemQuery =
                    this._containerRegistrations[containerToFind].Where(
                    c => c.Value.RegistrationType == TypeToFind);

                foreach (var item in itemQuery)
                    yield return item.Value;
            }
        }

        public void LoadFromConfiguration()
        {
            throw new NotImplementedException();
        }

        public IContainer Build(string RegistrationName = null)
        {
            IContainer container = null;

            if (string.IsNullOrEmpty(RegistrationName))
                container = (IContainer)Activator.CreateInstance(
                    this._containerType, this._containerRegistrations[DefaultContainerKey]);
            else
                container = (IContainer)Activator.CreateInstance(
                    this._containerType, this._containerRegistrations[RegistrationName]);

            return container;
        }

        private void InitializeFromStartup(Assembly AssemblyToRegister)
        {
            var startupAttributes = 
                AssemblyToRegister.GetCustomAttributes(typeof(StartupAttribute), false) as StartupAttribute[];

            if (startupAttributes != null && startupAttributes.Length > 0)
            {
                foreach (var startupAttribute in startupAttributes)
                {
                    IStartup startupInitializer =
                        Activator.CreateInstance(startupAttribute.StartupType) as IStartup;

                    if (startupInitializer == null)
                        continue;

                    startupInitializer.Initialize(this);
                }
            }
        }

        private void InitializeFromAssembly(Assembly AssemblyToRegister, string RegistrationName = null)
        {
            Type[] typesInAssembly = AssemblyToRegister.GetTypes();

            foreach (var type in typesInAssembly)
            {
                if (type.IsPublic && type.IsClass)
                {
                    var interfaces = type.GetInterfaces();
                    var name = string.IsNullOrEmpty(RegistrationName) ? DefaultContainerKey : RegistrationName;

                    foreach (var interfaceType in interfaces)
                        this.Register(type, name).As(interfaceType);
                }
            }
        }
    }
}
