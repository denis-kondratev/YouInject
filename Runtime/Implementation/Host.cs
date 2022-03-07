namespace YouInject
{
    public class Host : IHost
    {
        public IScope RootScope { get; }

        private readonly ServiceContainer _singletons;

        internal Host(ServiceCollection collection)
        {
            _singletons = new ServiceContainer();
            RootScope = new Scope(collection, _singletons);
        }

        public static IHostBuilder CreateBuilder()
        {
            var builder = new HostBuilder();

            return builder;
        }
        
        public void Dispose()
        {
            RootScope.Dispose();
            _singletons.Dispose();
        }
    }
}