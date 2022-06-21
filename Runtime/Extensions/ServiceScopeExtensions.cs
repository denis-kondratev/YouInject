namespace YouInject
{
    public static class ServiceScopeExtensions
    {
        public static void AddService<T>(this IServiceScope scope, object service)
        {
            var serviceType = typeof(T);
            scope.AddService(serviceType, service);
        }
    }
}