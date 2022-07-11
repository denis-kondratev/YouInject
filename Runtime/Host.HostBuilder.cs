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
                var serviceDescriptors = _services.Bake();
                var host = new Internal.Host(serviceDescriptors);
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