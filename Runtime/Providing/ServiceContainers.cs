using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ServiceContainers : IAsyncDisposable
    {
        private readonly IServiceContainer _singletonContainer;
        private readonly IServiceContainer _scopedContainer;
        private readonly bool _isDerived;
        private bool _isDisposed;

        private ServiceContainers()
        {
            _singletonContainer = new Container();
            _scopedContainer = new Container();
        }
        
        protected ServiceContainers(ServiceContainers parentContainers)
        {
            _isDerived = true;
            _singletonContainer = parentContainers._singletonContainer;
            _scopedContainer = parentContainers._scopedContainer.CreateDerivedContainer();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            await _scopedContainer.DisposeAsync();
            
            if (!_isDerived)
            {
                await _singletonContainer.DisposeAsync();
            }
        }

        internal static ServiceContainers CreateRootContainers()
        {
            var containers = new ServiceContainers();
            return containers;
        }

        internal ServiceContainers CreateDerivedContainers()
        {
            var derivedContainer = new ServiceContainers(this);
            return derivedContainer;
        }

        internal ComponentContainers CreateDerivedComponentContainers()
        {
            var derivedContainer = new ComponentContainers(this);
            return derivedContainer;
        }

        internal virtual void AddDecision(object decision, IServiceDescriptor descriptor)
        {
            Assert.IsNotNull(decision);
            
            if (TryGetContainer(descriptor.Lifetime, out var container))
            {
                container.AddDecision(decision, descriptor.ServiceType);
            }
        }

        internal bool TryGetDecision(IServiceDescriptor descriptor, [MaybeNullWhen(false)] out object decision)
        {
            if (TryGetContainer(descriptor.Lifetime, out var container))
            {
                return container.TryGetDecision(descriptor.ServiceType, out decision);
            }

            decision = null;
            return false;
        }

        private bool TryGetContainer(ServiceLifetime lifetime, [MaybeNullWhen(false)] out IServiceContainer container)
        {
            container = lifetime switch
            {
                ServiceLifetime.Transient => null,
                ServiceLifetime.Scoped => _scopedContainer,
                ServiceLifetime.Singleton => _singletonContainer,
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return container is not null;
        }
    }
}