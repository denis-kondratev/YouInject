namespace YouInject
{
    public interface IHostBuilder
    {
        IServiceCollection Services { get; }
        
        IHost BuildHost();

        void AddSceneLoader(ISceneLoader sceneLoader);
    }
}