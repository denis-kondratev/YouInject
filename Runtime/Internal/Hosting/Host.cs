using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal class Host : IHost
    {
        private readonly RootServiceScope _serviceScope;

        public Host(IReadOnlyDictionary<Type, IServiceDescriptor> descriptors)
        {
            _serviceScope = new RootServiceScope(descriptors);
            _serviceScope.AddService(typeof(IServiceScopeFactory), _serviceScope);
        }

        public IServiceProvider ServiceProvider => _serviceScope;
        
        public ValueTask DisposeAsync()
        {
            return _serviceScope.DisposeAsync();
        }
    }
}