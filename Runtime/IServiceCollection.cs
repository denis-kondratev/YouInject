namespace YouInject
{
    public interface IServiceCollection
    {
        void AddSingleton<TService, TDecision>();
        void AddSingleton<TService>();
        void AddScoped<TService, TDecision>();
        void AddScoped<TService>();
        IComponentDescriptorBuilder AddComponent<TService, TDecision>();
        IComponentDescriptorBuilder AddComponent<TService>();
        void AddFactory<TFactory, TDecision>();
    }
}