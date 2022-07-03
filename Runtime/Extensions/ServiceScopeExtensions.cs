using System;

namespace InjectReady.YouInject
{
    public static class ServiceScopeExtensions
    {
        public static void AddService<T>(this IServiceScope scope, object service)
        {
            var serviceType = typeof(T);
            scope.AddService(serviceType, service);
        }
        
        public static void InitializeService(this IServiceScope scope, object service, string methodName)
        {
            var serviceType = service.GetType();
            var methodInfo = serviceType.GetMethod(methodName);

            if (methodInfo is null)
            {
                throw new ArgumentException($"Cannot find the method {methodName} in the type {serviceType.FullName}");
            }
            
            scope.InitializeService(service, methodInfo);
        }
    }
}