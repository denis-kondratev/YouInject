namespace InjectReady.YouInject
{
    public interface IHostBuilder
    {
        IServiceCollection Services { get; }
        
        IHost BuildHost();
    }
}