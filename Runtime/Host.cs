using System;
using System.Threading.Tasks;
using InjectReady.YouInject.Internal;

namespace InjectReady.YouInject
{
    public static class Host
    {
        private static Internal.Host? _instance;

        public static IServiceScope RootScope
        {
            get
            {
                if (_instance is null)
                {
                    throw new InvalidOperationException("The host instance does not exist.");
                }
                
                return _instance.RootScope;
            }
        }
        
        public static IHostBuilder CreateBuilder()
        {
            return new HostBuilder();
        }

        public static ValueTask DisposeAsync()
        {
            if (_instance is null)
            {
                throw new InvalidOperationException("The host instance does not exist.");
            }

            return _instance.DisposeAsync();
        }

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
                return host;
            }
            
            private void AddBuiltInServices()
            {
                _services.AddSingleton<IYouInjectLogger, DefaultLogger>();
                _services.AddSingleton<Logger>();
                _services.AddDynamicSingleton<IServiceScopeFactory>();
            }
        }
    }
}