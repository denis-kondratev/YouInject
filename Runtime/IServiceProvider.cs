namespace YouInject
{
    public interface IServiceProvider
    {
        T Resolve<T>();
    }
}