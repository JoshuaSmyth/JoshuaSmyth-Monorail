using System;
using System.Collections.Generic;

namespace Monorail.Services
{
    public interface IService { }

    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, IService> InstantiatedServices = new Dictionary<Type, IService>();
        private static readonly Dictionary<Type, Func<IService>> ServiceFactory = new Dictionary<Type, Func<IService>>();
        private static readonly Dictionary<Type, Type> RemapType = new Dictionary<Type, Type>();

        public static bool IsInitalised { get; private set; }

        public static void Init()
        {
            foreach (var t in ServiceFactory)
            {
                // Due to instantiating services on demand we need to check if the instance exists as we iterate
                if (!InstantiatedServices.ContainsKey(t.Key))
                {
                    InstantiatedServices.Add(t.Key, t.Value());
                }
            }
            IsInitalised = true;
        }

        public static void RegisterService<T>(Func<IService> service) where T : IService
        {
            var type = typeof(T);
            if (ServiceFactory.ContainsKey(typeof(T)))
            {
                throw new Exception(String.Format("Error service:{0} already registered", type));
            }
            ServiceFactory.Add(typeof(T), service);
        }

        public static void RegisterService<T, S>(Func<IService> service) where T : IService
        {
            if (RemapType.ContainsKey(typeof(S)))
            {
                throw new Exception(String.Format("Error service mapping:{0} to {1} already registered!", typeof(S), typeof(T)));
            }

            RegisterService<T>(service);
            RemapType.Add(typeof(S), typeof(T));
        }

        public static void Clear()
        {
            ServiceFactory.Clear();
            RemapType.Clear();
        }

        /// <summary>
        /// Finds the requested service
        /// </summary>
        /// <returns>The service if found otherwise null</returns>
        public static T FindService<T>() where T : IService
        {
            var type = typeof(T);
            if (InstantiatedServices.ContainsKey(type))
            {
                return (T)InstantiatedServices[type];
            }

            // Check our remapping
            if (RemapType.ContainsKey(type))
            {
                type = RemapType[type];
                if (InstantiatedServices.ContainsKey(type))
                {
                    return (T)InstantiatedServices[type];
                }
            }

            // Could not find the service. Let's see if we should instanciate it
            if (ServiceFactory.ContainsKey(type))
            {
                var service = ServiceFactory[type]();
                InstantiatedServices.Add(type, service);
                return (T)service;
            }

            return default(T);
        }
    }
}