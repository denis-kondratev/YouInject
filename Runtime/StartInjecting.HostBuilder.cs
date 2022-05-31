using System;

namespace YouInject
{
    public static partial class HostUtility
    {
        private class HostBuilder : IHostBuilder
        {
            private const HostOptions DefaultOptions = HostOptions.UseLogs;
            private readonly ServiceCollection _services;
            private ISceneLoader? _sceneLoader;

            public HostBuilder()
            {
                _services = new ServiceCollection();
            }

            public IServiceCollection Services => _services;

            public IHost BuildHost()
            {
                AddBuiltInServices();
                
                var bakedServices = _services.Bake();
                _sceneLoader ??= new DefaultSceneLoader();
                var host = new Host(bakedServices, _sceneLoader, DefaultOptions);
                _host = host;
                return host;
            }

            public void AddSceneLoader(ISceneLoader sceneLoader)
            {
                if (_sceneLoader is not null)
                {
                    throw new Exception($"Trying to add {nameof(ISceneLoader)}, but it already exists");
                }

                _sceneLoader = sceneLoader;
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