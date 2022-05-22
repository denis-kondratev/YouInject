namespace YouInject
{
    public static partial class StartInjecting
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
                AddLogger();
                
                var bakedServices = _services.Bake();
                var host = new Host(bakedServices);
                return host;
            }

            private void AddLogger()
            {
                if (!_services.Contains<IYouInjectLogger>())
                {
                    _services.AddSingleton<IYouInjectLogger, DefaultLogger>();
                }
            }
        }
    }
}