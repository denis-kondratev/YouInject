using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal class Host : IHost
    {
        private readonly CacheableContainer _singletons;
        private readonly ServiceScope _serviceScope;

        public Host(IReadOnlyDictionary<Type, IServiceDescriptor> descriptors)
        {
            _singletons = new CacheableContainer();
            _serviceScope = new ServiceScope(_singletons, descriptors);
            _serviceScope.AddService(typeof(IServiceScopeFactory), _serviceScope);
        }

        public IServiceProvider ServiceProvider => _serviceScope;
        
        public async ValueTask DisposeAsync()
        {
            await _singletons.DisposeAsync();
        }
    }
}