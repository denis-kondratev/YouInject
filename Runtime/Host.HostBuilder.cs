using InjectReady.YouInject.Internal;

namespace InjectReady.YouInject
{
    public static partial class Host
    {
        private class HostBuilder : IHostBuilder
        {
            private readonly ServiceCollection _services;

            public HostBuilder()
            {
                _services = new ServiceCollection();
                AddBuiltInServices();
            }

            public IServiceCollection Services => _services;
            
            public IHost BuildHost()
            {
                _services.Bake();
                var rootScope = new RootServiceScope(_services.ServiceMap, _services.ComponentMap);
                var host = new Internal.Host(rootScope);
                _instance = host;
                return host;
            }
            
            private void AddBuiltInServices()
            {
                _services.AddDynamicService(typeof(IServiceScopeFactory), true);
            }
        }
    }
}