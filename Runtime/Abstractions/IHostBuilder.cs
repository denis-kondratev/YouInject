using System;

namespace InjectReady.YouInject
{
    public interface IHostBuilder
    {
        IHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);
        
        IHost BuildHost();
    }
}