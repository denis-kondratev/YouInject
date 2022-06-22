using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal class DisposableContainer : ServiceContainer
    {
        private List<object> _disposables;
        
        public DisposableContainer()
        {
            _disposables = new List<object>();
        }
        
        public override async ValueTask DisposeAsync()
        {
            if (IsDisposed) return;

            IsDisposed = true;

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

        public override object GetService(IServiceDescriptor descriptor, ContextualServiceProvider serviceProvider)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            ThrowIfDisposed();
            
            var service = CreateService(descriptor, serviceProvider);
            CaptureDisposable(service);
            return service;
        }
        
        private void CaptureDisposable(object service)
        {
            if (service is IDisposable or IAsyncDisposable)
            {
                _disposables.Add(service);
            }
        }
    }
}