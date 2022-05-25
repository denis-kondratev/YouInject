namespace YouInject
{
    public interface IServiceCollection
    {
        IServiceDescriptorBuilder AddSingleton<TService, TDecision>();
        IServiceDescriptorBuilder AddSingleton<TService>();
        IServiceDescriptorBuilder AddScoped<TService, TDecision>();
        IServiceDescriptorBuilder AddScoped<TService>();
        IComponentDescriptorBuilder AddComponent<TService, TDecision>();
        IComponentDescriptorBuilder AddComponent<TService>();
        void AddFactory<TFactory, TDecision>();
        bool Contains<TService>();
    }
}