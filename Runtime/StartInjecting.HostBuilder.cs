using System;

namespace YouInject
{
    public static partial class HostUtility
    {
        private class HostBuilder : IHostBuilder
        {
            private const HostOptions DefaultOptions = HostOptions.UseLogs;
            private readonly ServiceCollection _services;

            public HostBuilder()
            {
                _services = new ServiceCollection();
            }

            public IServiceCollection Services => _services;

            public IHost BuildHost()
            {
                AddBuiltInServices();
                
                var bakedServices = _services.Bake();
                var host = new Host(bakedServices, DefaultOptions);
                _host = host;
                return host;
            }

            private void AddBuiltInServices()
            {
                if (!_services.Contains<IYouInjectLogger>())
                {
                    _services.AddSingleton<IYouInjectLogger, DefaultLogger>();
                }
                
                _services.AddScoped<IScope, ServiceScope>().AsComponent();
                _services.AddSingleton<IHost, Host>();
            }
        }
    }
}