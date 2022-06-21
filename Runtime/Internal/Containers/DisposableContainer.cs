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

        public override object GetService(IServiceDescriptor descriptor, ScopeContext context)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (context == null) throw new ArgumentNullException(nameof(context));

            ThrowIfDisposed();
            
            var service = CreateService(descriptor, context);
            CaptureDisposable(service);
            return service;
        }

        public void AddService(IServiceDescriptor descriptor, object service)
        {
            throw new InvalidOperationException("Transient service cannot be added.");
        }

        public virtual void RemoveService(IServiceDescriptor descriptor)
        {
            throw new InvalidOperationException("Transient service cannot be removed.");
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