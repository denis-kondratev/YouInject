using System;

namespace InjectReady.YouInject
{
    public interface IHostBuilder
    {
        IHostBuilder RegisterServices(Action<IServiceCollection> registerServices);
        
        IHost BuildHost();
    }
}