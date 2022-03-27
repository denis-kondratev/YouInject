using System.Diagnostics.CodeAnalysis;

namespace YouInject
{
    public static class InjectionHub
    {
        [AllowNull] public static IHost BuiltHost { get; private set; }

        public static IHostBuilder CreateBuilder()
        {
            var builder = new HostBuilder();
            return builder;
        }
        
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