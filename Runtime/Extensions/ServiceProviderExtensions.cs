using System;

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
    }
}