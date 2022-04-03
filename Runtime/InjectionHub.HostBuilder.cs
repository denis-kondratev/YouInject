namespace YouInject
{
    public static partial class InjectionHub
    {
        private class HostBuilder : IHostBuilder
        {
            private readonly ServiceCollection _services;
        
            public IServiceCollection Services => _services;

            public HostBuilder()
            {
                _services = new ServiceCollection();
            }

            public IHost BuildHost()
            {
                var bakedServices = _services.Bake();
                var host = new Host(bakedServices);
                BuiltHost = host;
                return host;
            }
        }
    }
}