namespace YouInject.Internal
{
    internal interface IDynamicDescriptor : IServiceDescriptor
    {
        void SetLifetime(ServiceLifetime lifetime);
    }
}