using System;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceScope : IServiceScope, IExtendedServiceProvider
    {
        private readonly ServiceProvider _provider;
        private readonly ScopeContext _context;
        private bool _isDisposed;

        public IExtendedServiceProvider ServiceProvider => this;
        
        public ServiceScope(ServiceProvider provider)
        {
            _provider = provider;
            _context = new ScopeContext();
        }

        public ValueTask DisposeAsync()
        {
            if (_isDisposed) return default;

            _isDisposed = true;
            return _context.DisposeAsync();
        }
        
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfDisposed();
            
            return _provider.GetService(serviceType, _context);
        }

        public void AddDynamicService(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            
            ThrowIfDisposed();
            
            _provider.AddDynamicService(serviceType, instance, _context);
        }

        public void InitializeComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        public void StockpileComponent(MonoBehaviour component)
        {
            throw new NotImplementedException();
        }
        
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ServiceProvider));
            }
        }
    }
}