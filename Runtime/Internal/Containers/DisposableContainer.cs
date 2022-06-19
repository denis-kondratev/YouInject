using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal class DisposableContainer : IServiceContainer
    {
        private List<object> _disposables;
        private bool _isDisposed;
        
        public DisposableContainer()
        {
            _disposables = new List<object>();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            foreach (var disposable in _disposables)
            {
                if (disposable is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    ((IDisposable)disposable).Dispose();
                }
            }
            
            _disposables = null!;
        }

        public virtual object GetService(IServiceDescriptor descriptor, ServiceScope.Context context)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (context == null) throw new ArgumentNullException(nameof(context));

            ThrowIfDisposed();
            
            var service = CreateService(descriptor, context);
            CaptureDisposable(service);
            return service;
        }

        protected static object CreateService(IServiceDescriptor descriptor, ServiceScope.Context context)
        {
            var service = descriptor.InstanceFactory.Invoke(context);
            return service;
        }
        
        protected void CaptureDisposable(object service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            
            ThrowIfDisposed();

            if (service is IDisposable or IAsyncDisposable)
            {
                _disposables.Add(service);
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_isDisposed) throw new InvalidOperationException("Container is already disposed.");
        }
    }
}