using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSingleton<TService, TInstance>(this IServiceCollection services)
        {
            services.AddService<TService, TInstance>(ServiceLifetime.Singleton);
        }
        
        public static void AddSingleton<TService>(this IServiceCollection services)
        {
            services.AddService<TService>(ServiceLifetime.Singleton);
        }
        
        public static void AddScoped<TService, TInstance>(this IServiceCollection services)
        {
            services.AddService<TService, TInstance>(ServiceLifetime.Scoped);
        }
        
        public static void AddScoped<TService>(this IServiceCollection services)
        {
            services.AddService<TService>(ServiceLifetime.Scoped);
        }
        
        public static void AddTransient<TService, TInstance>(this IServiceCollection services)
        {
            services.AddService<TService, TInstance>(ServiceLifetime.Transient);
        }
        
        public static void AddTransient<TService>(this IServiceCollection services)
        {
            services.AddService<TService>(ServiceLifetime.Transient);
        }

        public static void AddDelegateFactory<TDelegate, TProduct>(this IServiceCollection services) 
            where TDelegate : Delegate 
        {
            var delegateType = typeof(TDelegate);
            var productType = typeof(TProduct);
            services.AddDelegateFactory(delegateType, productType, ServiceLifetime.Scoped);
        }

        public static DynamicServiceRegistration AddDynamicService<TService>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            return services.AddDynamicService(serviceType);
        }

        public static void AddMonoBehaviourInitialization<T>(
            this IServiceCollection services,
            string initializingMethodName) where T : MonoBehaviour
        {
            var monoBehaviourType = typeof(T);
            services.InitializeComponentWith(monoBehaviourType, initializingMethodName);
        }
        
        private static void AddService<TService, TInstance>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var serviceType = typeof(TService);
            var instanceType = typeof(TInstance);
            services.AddService(serviceType, instanceType, lifetime);
        }

        private static void AddService<TService>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var serviceType = typeof(TService);
            services.AddService(serviceType, serviceType, lifetime);
        }
    }
}