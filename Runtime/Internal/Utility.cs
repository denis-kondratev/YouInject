using System;
using System.Reflection;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal static class Utility
    {
        public static bool IsComponentType(Type value)
        {
            return value.IsSubclassOf(typeof(MonoBehaviour)) && !value.IsAbstract;
        }

        public static object[] GetServices(this Func<Type, object> serviceProvider, ParameterInfo[] serviceInfos)
        {
            return GetServices(serviceProvider, serviceInfos, info => info.ParameterType);
        }
        
        public static object[] GetServices(this Func<Type, object> serviceProvider, Type[] serviceTypes)
        {
            return GetServices(serviceProvider, serviceTypes, type => type);
        }

        private static object[] GetServices<T>(
            this Func<Type, object> serviceProvider,
            T[] typeSources,
            Func<T, Type> typeProvider)
        {
            if (typeSources.Length == 0)
            {
                return Array.Empty<object>();
            }

            var services = new object[typeSources.Length];
            
            for (var i = 0; i < typeSources.Length; i++)
            {
                services[i] = serviceProvider.Invoke(typeProvider.Invoke(typeSources[i]));
            }

            return services;
        }
    }
}