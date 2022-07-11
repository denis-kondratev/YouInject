using System;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal class Host : IHost
    {
        private readonly RootServiceScope _rootScope;
        private bool _isDisposed;

        public IExtendedServiceProvider ServiceProvider
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("Host has already been disposed of.");
                }
                
                return _rootScope;
            }
        }
        
        public Host(RootServiceScope rootScope)
        {
            _rootScope = rootScope;
            _rootScope.AddDynamicService(typeof(IServiceScopeFactory), _rootScope);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            await _rootScope.DisposeAsync();
        }
    }
}