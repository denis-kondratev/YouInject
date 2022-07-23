namespace InjectReady.YouInject.Internal
{
    internal interface IConstructableServiceDescriptor : IServiceDescriptor
    {
        ServiceLifetime Lifetime { get; }
        object ResolveService(ServiceProvider serviceProvider, ScopeContext scopeContext);
    }
}