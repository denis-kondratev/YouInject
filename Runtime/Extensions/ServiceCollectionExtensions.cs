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

        public static void AddFactory<TFactory, TProduct>(this IServiceCollection services)
        {
            var factoryType = typeof(TFactory);
            var productType = typeof(TProduct);
            //services.AddDelegateFactory(factoryType, productType, ServiceLifetime.Scoped);
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
        
        public static void AddSingletonComponent<TService>(this IServiceCollection services, string? initializingMethodName = null)
        {
            var serviceType = typeof(TService);
            //services.AddDynamicComponent(serviceType, true, initializingMethodName);
        }
        
        public static void AddScopedComponent<TService>(this IServiceCollection services, string? initializingMethodName = null)
        {
            var serviceType = typeof(TService);
            //services.AddDynamicComponent(serviceType, false, initializingMethodName);
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