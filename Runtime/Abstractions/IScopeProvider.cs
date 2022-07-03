namespace InjectReady.YouInject
{
    public interface IScopeProvider
    {
        IServiceScope CreateScope(string scopeId);
    }
}