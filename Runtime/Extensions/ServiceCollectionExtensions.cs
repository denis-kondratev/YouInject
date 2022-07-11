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

        public static void AddDynamicSingleton<TService>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            services.AddDynamicService(serviceType, true);
        }
        
        public static void AddDynamicScoped<TService>(this IServiceCollection services)
        {
            var serviceType = typeof(TService);
            services.AddDynamicService(serviceType, false);
        }

        public static void AddScopeMonoBehaviour<TService, TMonoBehaviour>(
            this IServiceCollection services,
            string? initializingMethodName = null) where TMonoBehaviour : MonoBehaviour
        {
            AddMonoBehaviour<TService, TMonoBehaviour>(services, false, initializingMethodName);
        }
        
        public static void AddScopeMonoBehaviour<TMonoBehaviour>(
            this IServiceCollection services,
            string? initializingMethodName = null) where TMonoBehaviour : MonoBehaviour
        {
            AddMonoBehaviour<TMonoBehaviour, TMonoBehaviour>(services, false, initializingMethodName);
        }
        
        public static void AddSingletonMonoBehaviour<TService, TMonoBehaviour>(
            this IServiceCollection services,
            string? initializingMethodName = null) where TMonoBehaviour : MonoBehaviour
        {
            AddMonoBehaviour<TService, TMonoBehaviour>(services, true, initializingMethodName);
        }
        
        public static void AddSingletonMonoBehaviour<TMonoBehaviour>(
            this IServiceCollection services,
            string? initializingMethodName = null) where TMonoBehaviour : MonoBehaviour
        {
            AddMonoBehaviour<TMonoBehaviour, TMonoBehaviour>(services, true, initializingMethodName);
        }

        public static void AddMonoBehaviourInitialization<T>(
            this IServiceCollection services,
            string initializingMethodName) where T : MonoBehaviour
        {
            var monoBehaviourType = typeof(T);
            services.AddMonoBehaviourInitialization(monoBehaviourType, initializingMethodName);
        }
        
        private static void AddMonoBehaviour<TService, TMonoBehaviour>(
            this IServiceCollection services,
            bool isSingleton,
            string? initializingMethodName = null) where TMonoBehaviour : MonoBehaviour
        {
            var serviceType = typeof(TService);
            var monoBehaviourType = typeof(TMonoBehaviour);

            if (!serviceType.IsAssignableFrom(monoBehaviourType))
            {
                throw new InvalidOperationException(
                    $"The '{serviceType.FullName}' service is not assignable from '{monoBehaviourType.FullName}' type");
            }

            services.AddDynamicService(serviceType, isSingleton);
            services.BindMonoBehaviourToService(monoBehaviourType, serviceType);
            
            if (!string.IsNullOrEmpty(initializingMethodName))
            {
                services.AddMonoBehaviourInitialization(monoBehaviourType, initializingMethodName);
            }
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