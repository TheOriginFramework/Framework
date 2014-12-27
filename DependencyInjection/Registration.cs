using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public class Registration
    {
        public Type RegistrationType { get; private set; }
        public Type MapInterfaceType { get; private set; }
        public Type MapConcreteType { get; private set; }
        public List<Type> GenericTypes { get; private set; }
        public bool IsDefault { get; private set; }
        public bool IsSingletonInstance { get; private set; }
        public object RegisteredInstance { get; private set; }

        public Registration(Type RegistrationType)
        {
            this.RegistrationType = RegistrationType;
            this.IsSingletonInstance = false;
        }

        public Registration As<TInterface>()
        {
            this.MapInterfaceType = typeof(TInterface);
            return this;
        }

        public Registration As(Type InterfaceType)
        {
            if (InterfaceType == null)
                throw new ArgumentNullException("ERROR_TYPE_NOT_FOUND");
            if (!InterfaceType.IsInterface)
                throw new ArgumentException("ERROR_ILLEGAL_TYPE");

            this.MapInterfaceType = InterfaceType;
            return this;
        }

        public Registration AsClass<TClass>()
        {
            this.MapConcreteType = typeof(TClass);
            return this;
        }

        public Registration AsClass(Type ClassType)
        {
            if (ClassType == null)
                throw new ArgumentNullException("ERROR_TYPE_NOT_FOUND");
            if (ClassType.IsInterface)
                throw new ArgumentException("ERROR_ILLEGAL_TYPE");

            this.MapConcreteType = ClassType;
            return this;
        }

        public Registration ForGeneric<TGenericType>()
        {
            if (this.GenericTypes.Contains(typeof(TGenericType)))
                return this;

            this.GenericTypes.Add(typeof(TGenericType));
            return this;
        }

        public Registration ForGeneric(Type GenericType)
        {
            if (this.GenericTypes.Contains(GenericType))
                return this;

            this.GenericTypes.Add(GenericType);
            return this;
        }

        public Registration AsDefault()
        {
            this.IsDefault = true;
            return this;
        }

        public Registration AsSingleInstance()
        {
            this.IsSingletonInstance = true;
            return this;
        }

        public Registration WithInstance(object Instance)
        {
            this.RegisteredInstance = Instance;
            return this;
        }

        public Registration WithInstance(Expression<Func<IContainer, object>> InstanceFromResolver)
        {
            object o = Expression.Lambda(InstanceFromResolver.Body).Compile().DynamicInvoke();
            this.RegisteredInstance = o;
            return this;
        }
    }
}
