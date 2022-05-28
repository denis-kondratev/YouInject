namespace YouInject
{
    internal class ServiceScope : Scope
    {
        private bool _isDisposed;
        
        public ServiceScope(BakedServiceCollection services, ServiceProvider serviceProvider, string name, Scope? parentScope) 
            : base (services, serviceProvider, name, parentScope)
        {
        }
    }
}