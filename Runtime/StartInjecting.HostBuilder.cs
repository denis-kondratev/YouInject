using System;

namespace YouInject
{
    public static partial class HostUtility
    {
        private class HostBuilder : IHostBuilder
        {
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
                var host = new Host(bakedServices);
                _host = host;
                return host;
            }

            private void AddBuiltInServices()
            {
                if (!_services.Contains<IYouInjectLogger>())
                {
                    _services.AddSingleton<IYouInjectLogger, DefaultLogger>();
                }
                
                if (_services.Contains<IScope>())
                {
                    throw new Exception();
                }
                
                _services.AddScoped<IScope, Scope>().AsComponent();
            }
        }
    }
}