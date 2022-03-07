namespace YouInject
{
    public interface IServiceProvider
    {
        T GetService<T>() where T : class;
    }
}