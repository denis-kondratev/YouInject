using System;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal class Host : IHost
    {
        private readonly ServiceProvider _serviceProvider;
        private bool _isDisposed;

        public IExtendedServiceProvider ServiceProvider
        {
            get
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(IHost));
                }
                
                return _serviceProvider;
            }
        }
        
        public Host(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serviceProvider.AddDynamicService(typeof(IServiceScopeFactory), _serviceProvider);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            await _serviceProvider.DisposeAsync();
        }
    }
}