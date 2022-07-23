using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ScopeContext : IAsyncDisposable
    {
        private readonly List<object> _disposables;
        private readonly Dictionary<Type, object> _cachedServices;
        private readonly Dictionary<Type, MonoBehaviour> _stockpile;
        private bool _isDisposed;
        
        internal ScopeContext()
        {
            _cachedServices = new Dictionary<Type, object>();
            _disposables = new List<object>();
            _stockpile = new Dictionary<Type, MonoBehaviour>();
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            
            foreach (var service in _disposables)
            {
                if (service is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    ((IDisposable)service).Dispose();
                }
            }
        }
        
        internal bool TryGetCachedService(Type serviceType, out object service)
        {
            ThrowIfDisposed();

            return _cachedServices.TryGetValue(serviceType, out service);
        }
        
        internal void CaptureService(object service)
        {
            ThrowIfDisposed();
            
            if (service is IDisposable or IAsyncDisposable)
            {
                _disposables.Add(service);
            }
        }
        
        internal void CacheService(object instance, Type serviceType)
        {
            ThrowIfDisposed();
            
            if (instance is IDisposable or IAsyncDisposable)
            {
                _disposables.Add(instance);
            }
            
            if (!_cachedServices.TryAdd(serviceType, instance))
            {

                throw new InvalidOperationException(
                    $"Cannot cache an instance of the '{serviceType.Name}' service. "
                    + "Another one has already been added.");
            }
        }
        
        internal void StockpileComponent(MonoBehaviour component)
        {
            ThrowIfDisposed();
            var componentType = component.GetType();
            
            if (!_stockpile.TryAdd(componentType, component))
            {
                throw new InvalidOperationException(
                    $"Cannot stockpile the '{componentType.Name}' component. "
                    + "A component with the same type has already been stockpiled.");
            }
        }
        
        internal MonoBehaviour PickUpComponent(Type componentType)
        {
            ThrowIfDisposed();
            
            if (_stockpile.Remove(componentType, out var component))
            {
                return component;
            }

            throw new InvalidOperationException(
                $"Cannot get the '{componentType}' component from the stockpile. "
                + "It has not added yet");
        }
        
        internal bool TryPickUpComponent(Type componentType, [MaybeNullWhen(false)] out MonoBehaviour component)
        {
            ThrowIfDisposed();
            
            return _stockpile.Remove(componentType, out component);
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