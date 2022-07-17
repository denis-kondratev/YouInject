using System;
using InjectReady.YouInject.Internal;

namespace InjectReady.YouInject
{
    public static partial class Host
    {
        private class HostBuilder : IHostBuilder
        {
            private readonly ServiceCollection _services;

            internal HostBuilder()
            {
                _services = new ServiceCollection();
                AddBuiltInServices();
            }

            public IHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
            {
                if (configureServices == null) throw new ArgumentNullException(nameof(configureServices));
                
                configureServices.Invoke(_services);
                return this;
            }

            public IHost BuildHost()
            {
                _services.Bake();
                var serviceProvider = new ServiceProvider(_services.ServiceMap, _services.ComponentMap);
                var host = new Internal.Host(serviceProvider);
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