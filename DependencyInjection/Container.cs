using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public class Container : IContainer
    {
        private static Dictionary<Type, object> gSingleInstanceContainer = new Dictionary<Type, object>();
        private Dictionary<Type, Registration> _serviceRegistrations = null;
        private object _synRoot = new object();

        public Container(IDictionary<Type, Registration> Registrations)
        {
            this._serviceRegistrations = new Dictionary<Type, Registration>(Registrations);
        }

        public TInterface Resolve<TInterface>()
        {
            return (TInterface)this.Resolve(typeof(TInterface));
        }

        public TInterface Resolve<TInterface>(params object[] ConstructorParams)
        {
            return (TInterface)this.Resolve(typeof(TInterface), ConstructorParams);
        }

        public TInterface Resolve<TInterface, TConcreteClass>() where TConcreteClass : TInterface
        {
            return (TInterface)this.Resolve(typeof(TInterface), typeof(TConcreteClass));
        }

        public TInterface Resolve<TInterface, TConcreteClass>(params object[] ConstructorParams) where TConcreteClass : TInterface
        {
            return (TInterface)this.Resolve(typeof(TInterface), typeof(TConcreteClass), ConstructorParams);
        }

        public object Resolve(Type InterfaceType, params object[] ConstructorParams)
        {
            var registration = this.GetComponentRegistration(InterfaceType);

            if (registration == null)
                throw new RegistrationNotFoundException();

            return this.CreateInstance(registration, ConstructorParams);
        }
        
        public object Resolve(Type InterfaceType, Type ConcreteType, params object[] ConstructorParams)
        {
            var registration = this.GetComponentRegistration(InterfaceType, ConcreteType);

            if (registration == null)
                throw new RegistrationNotFoundException();

            return this.CreateInstance(registration, ConstructorParams);
        }

        public ILifetimeManager BeginLifetimeManagement()
        {
            throw new NotImplementedException();
        }
        private object CreateInstance(Registration Registration, params object[] ConstructorParams)
        {
            Type registeredType = Registration.RegistrationType;

            if (Registration.IsSingletonInstance)
            {
                if (!gSingleInstanceContainer.ContainsKey(registeredType))
                {
                    lock (this._synRoot)
                    {
                        try
                        {
                            if (!gSingleInstanceContainer.ContainsKey(registeredType))
                            {
                                if (ConstructorParams == null)
                                {
                                    gSingleInstanceContainer.Add(
                                        registeredType,
                                        Activator.CreateInstance(registeredType));
                                }
                                else
                                {
                                    gSingleInstanceContainer.Add(
                                        registeredType,
                                        Activator.CreateInstance(registeredType,
                                        ConstructorParams.ToArray()));
                                }
                            }
                        }
                        catch (MissingMethodException)
                        {
                            throw new TypeRegistrationException();
                        }
                    }
                }

                return gSingleInstanceContainer[registeredType];
            }
            else
            {
                try
                {
                    if (ConstructorParams == null)
                        return Activator.CreateInstance(registeredType);
                    else
                        return Activator.CreateInstance(registeredType, ConstructorParams.ToArray());
                }
                catch (MissingMethodException)
                {
                    throw new TypeRegistrationException();
                }
            }
        }

        private Registration GetComponentRegistration(Type TypeToResolve, Type ConcreteType = null)
        {
            IEnumerable<KeyValuePair<Type, Registration>> registrationQuery = null;

            if (TypeToResolve.IsInterface)
            {
                if (ConcreteType == null)
                    registrationQuery = this._serviceRegistrations.Where(
                        c => c.Value.MapInterfaceType == TypeToResolve);
                else
                    registrationQuery = this._serviceRegistrations.Where(
                        c => c.Value.MapInterfaceType == TypeToResolve && c.Key == ConcreteType);
            }
            else
            {
                // search base class first.
                if (ConcreteType == null)
                    registrationQuery = this._serviceRegistrations.Where(
                        c => c.Value.MapInterfaceType == TypeToResolve);
                else
                    registrationQuery = this._serviceRegistrations.Where(
                        c => c.Value.MapInterfaceType == TypeToResolve && c.Key == ConcreteType);

                // if not base type, search concrete type.
                if (!registrationQuery.Any())
                {
                    if (ConcreteType == null)
                        registrationQuery = this._serviceRegistrations.Where(
                            c => c.Key == TypeToResolve);
                    else
                        registrationQuery = this._serviceRegistrations.Where(
                            c => c.Key == TypeToResolve && c.Key == ConcreteType);
                }
            }

            if (!registrationQuery.Any())
                return null;
            else if (registrationQuery.Count() > 1)
            {
                Registration registration = null;

                foreach (var item in registrationQuery)
                {
                    if (item.Value.IsDefault)
                    {
                        registration = item.Value;
                        break;
                    }
                }

                if (registration == null)
                    registration = registrationQuery.First().Value;

                return registration;
            }
            else
                return registrationQuery.First().Value;
        }
    }
}
