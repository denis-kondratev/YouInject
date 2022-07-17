using System;
using System.Reflection;
using InjectReady.YouInject.Internal;

namespace InjectReady.YouInject
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider services)
        {
            var serviceType = typeof(T);
            var instance = services.GetService(serviceType);
            return (T)instance;
        }

        public static void AddDynamicService<T>(this IExtendedServiceProvider services, object instance)
        {
            var serviceType = typeof(T);
            services.AddDynamicService(serviceType, instance);
        }

        public static IServiceScope CreateScope(this IServiceProvider services)
        {
            var scopeFactory = services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            return scope;
        }

        internal static void GetServices(this ServiceProvider provider, ScopeContext context, Type[] serviceTypes, object[] services)
        {
            GetServices(provider, context, serviceTypes, t => t, services);
        }
        
        internal static void GetServices(this ServiceProvider provider, ScopeContext context, ParameterInfo[] serviceInfos, object[] services)
        {
            GetServices(provider, context, serviceInfos, i => i.ParameterType, services);
        }

        private static void GetServices<T>(this ServiceProvider provider,
            ScopeContext context,
            T[] typeProviders,
            Func<T, Type> getType,
            object[] services)
        {
            for (var i = 0; i < services.Length; i++)
            {
                services[i] = provider.GetService(getType(typeProviders[i]), context);
            }
        }
    }
}