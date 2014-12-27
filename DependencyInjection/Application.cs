using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public static class Application
    {
        private static IContainerBuilder gContainerBuilder = null;

        static Application()
        {
            gContainerBuilder = new ContainerBuilder();
        }

        public static void Initialize(Action<IContainerBuilder> Initializer)
        {
            Initializer(gContainerBuilder);
        }

        public static IContainer GetServices(string RegistrationName = "DEFAULT")
        {
            return gContainerBuilder.Build(RegistrationName);
        }

        public static IEnumerable<Registration> GetTypeRegistrations<T>()
        {
            return gContainerBuilder.Find<T>();
        }

        public static IEnumerable<Registration> GetConcreteTypeRegistrations<T>()
        {
            return gContainerBuilder.FindByConcrete<T>();
        }

        public static IEnumerable<Registration> GetTypeRegistrations(Type TypeToGet)
        {
            return gContainerBuilder.Find(TypeToGet);
        }

        public static IEnumerable<Registration> GetConcreteTypeRegistrations(Type TypeToGet)
        {
            return gContainerBuilder.FindByConcrete(TypeToGet);
        }
    }
}
