using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal class Host : IHost
    {
        private readonly RootServiceScope _serviceScope;
        private bool _isDisposed;

        public IExtendedServiceProvider ServiceProvider
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("Host has already been disposed of.");
                }
                
                return _serviceScope;
            }
        }
        
        public Host(IReadOnlyDictionary<Type, IServiceDescriptor> descriptors)
        {
            _serviceScope = new RootServiceScope(descriptors);
            _serviceScope.AddDynamicService(typeof(IServiceScopeFactory), _serviceScope);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            await _serviceScope.DisposeAsync();
        }
    }
}