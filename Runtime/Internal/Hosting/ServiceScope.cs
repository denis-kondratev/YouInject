using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InjectReady.YouInject.Internal
{
    internal abstract partial class ServiceScope : IServiceScope, IContextualScope, INotifyOnDisposed<ServiceScope>
    {
        protected readonly CachingContainer ScopedContainer;
        protected readonly DisposableContainer TransientContainer;
        private readonly Stack<ContextualServiceProvider> _contextPool;
        private bool _isDisposed;
        private readonly List<ServiceScope> _derivedScopes;

        public event Action<ServiceScope>? Disposed;
        
        protected ServiceScope()
        {
            ScopedContainer = new CachingContainer();
            TransientContainer = new DisposableContainer();
            _contextPool = new Stack<ContextualServiceProvider>();
            _derivedScopes = new List<ServiceScope>();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            
            foreach (var derivedScope in _derivedScopes)
            {
                await derivedScope.DisposeAsync();
            }

            await ScopedContainer.DisposeAsync();
            await TransientContainer.DisposeAsync();
            
            Disposed?.Invoke(this);
        }
        
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfDisposed();
            
            using var context = new Context(this);
            var service = context.ServiceProvider.GetService(serviceType);
            return service;
        }

        public void AddService(Type serviceType, object service)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (service == null) throw new ArgumentNullException(nameof(service));

            ThrowIfDisposed();
            ThrowIfServiceDoesNotMatchType(serviceType, service);

            if (GetDescriptor(serviceType) is not DynamicDescriptor descriptor)
            {
                throw new InvalidOperationException($"The '{serviceType.Name}' service is not registered as dynamic one.");
            }

            var container = GetContainer(descriptor.Lifetime) as CachingContainer;
            
            if (container!.Contains(serviceType))
            {
                throw new InvalidOperationException($"Cannot add the service '{serviceType.Name}'. It already exists.");
            }

            OnAddingComponent(descriptor, service);
            container.AddService(serviceType, service);
        }

        public void AddService<T>(object service)
        {
            var serviceType = typeof(T);
            AddService(serviceType, service);
        }

        private void OnAddingComponent(DynamicDescriptor descriptor, object service)
        {
            if (descriptor is not ComponentDescriptor) return;
            
            if (service is not MonoBehaviour component)
            {
                throw new ArgumentException(
                    $"Cannot dynamically add the service '{descriptor.ServiceType.FullName}'. The service is " +
                    $"registered as Component but it is not derived from '{typeof(MonoBehaviour).FullName}'.",
                    nameof(service));
            }

            if (descriptor.Lifetime is ServiceLifetime.Singleton)
            {
                Object.DontDestroyOnLoad(component);
            }
        }

        public void RemoveService(Type serviceType)
        {
            if (_isDisposed) return;

            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            var descriptor = GetDescriptor(serviceType);

            if (descriptor is not DynamicDescriptor)
            {
                throw new InvalidOperationException($"Cannot delete '{serviceType.Name}' service. " +
                                                    "Only dynamic services can be deleted.");
            }
            
            var container = GetContainer(descriptor.Lifetime) as CachingContainer;
            container!.RemoveService(descriptor.ServiceType);
        }
        
        public void InitializeService(Type serviceType, object service)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (service == null) throw new ArgumentNullException(nameof(service));
            
            ThrowIfDisposed();

            if (GetDescriptor(serviceType) is not ComponentDescriptor descriptor)
            {
                throw new InvalidOperationException(
                    $"Cannot initialize service of type '{serviceType.FullName}'. " +
                    "It must be registered as Component.");
            }

            var implementationType = service.GetType();
            var initializingMethod = descriptor.GetInitializingMethod(implementationType);

            if (initializingMethod is null)
            {
                throw new InvalidOperationException(
                    $"Cannot initialize service of type '{serviceType.FullName}'. " +
                    "There is no registered initializing method.");
            }
            
            InitializeService(service, initializingMethod);
        }

        public void InitializeService<T>(T service) where T : MonoBehaviour
        {
            var serviceType = typeof(T);
            InitializeService(serviceType, service);
        }

        public void InitializeService(object service, MethodInfo methodInfo)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            
            ThrowIfDisposed();

            using var context = new Context(this);
            var parameterTypes = methodInfo.GetParameters();
            var parameters = context.ServiceProvider.GetServices(parameterTypes);
            methodInfo.Invoke(service, parameters);
        }

        public void InitializeService(object service, string methodName)
        {
            var serviceType = service.GetType();
            var methodInfo = serviceType.GetMethod(methodName);

            if (methodInfo is null)
            {
                throw new ArgumentException($"Cannot find the method {methodName} in the type {serviceType.FullName}");
            }
            
            InitializeService(service, methodInfo);
        }

        public IServiceScope CreateScope()
        {
            var scopeFactory = this.GetService<IServiceScopeFactory>();
            var derivedScope = scopeFactory.CreateScope();
            _derivedScopes.Add(derivedScope);

            derivedScope.Disposed += scope =>
            {
                if (_isDisposed) return;

                _derivedScopes.Remove(scope);
            };

            return derivedScope;
        }

        public abstract IServiceContainer GetContainer(ServiceLifetime lifetime);

        public abstract IServiceDescriptor GetDescriptor(Type serviceType);

        public abstract bool TryGetDescriptor(Type serviceType, [MaybeNullWhen(false)] out IServiceDescriptor descriptor);

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Containers is already disposed");
            }
        }
        
        private static void ThrowIfServiceDoesNotMatchType(Type serviceType, object service)
        {
            if (!serviceType.IsInstanceOfType(service))
            {
                throw new ArgumentException(
                    $"The service of type '{service.GetType().Name}' is not instance of type '{serviceType.Name}'.",
                    nameof(service));
            }
        }
    }
}