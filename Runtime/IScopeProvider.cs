namespace YouInject
{
    public interface IScopeProvider
    {
        IServiceScope CreateScope(string scopeId);
    }
}