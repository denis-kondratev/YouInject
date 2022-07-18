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

            public IHostBuilder RegisterServices(Action<IServiceCollection> registerServices)
            {
                if (registerServices == null) throw new ArgumentNullException(nameof(registerServices));
                
                registerServices.Invoke(_services);
                return this;
            }

            public IHost BuildHost()
            {
                _services.Bake();
                var serviceProvider = new ServiceProvider(_services.ServiceMap, _services.ComponentDescriptors);
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