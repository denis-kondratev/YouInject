namespace InjectReady.YouInject.Internal
{
    internal class HostBuilder : IHostBuilder
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
            var services = _services.Bake();
            var host = new global::InjectReady.YouInject.Internal.Host(services);
            return host;
        }
            
        private void AddBuiltInServices()
        {
            _services.AddSingleton<IYouInjectLogger, DefaultLogger>();
            _services.AddSingleton<Logger>();
            _services.AddDynamic<IServiceScopeFactory>();
        }
    }
}