namespace InjectReady.YouInject.Internal
{
    internal interface IServiceScopeFactory
    {
        ServiceScope CreateScope(ThruContainer scopedContainer);
    }
}