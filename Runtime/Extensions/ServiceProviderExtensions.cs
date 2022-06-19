using System;

namespace YouInject
{
    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider services)
        {
            var serviceType = typeof(T);
            var instance = services.GetService(serviceType);
            return (T)instance;
        }
        
        public static IServiceScope CreateScope(this IServiceProvider services)
        {
            var scopeFactory = services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            return scope;
        }
    }
}