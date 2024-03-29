﻿using System;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceScope : IServiceScope
    {
        private readonly ServiceProvider _provider;
        private readonly ScopeContext _context;
        private bool _isDisposed;
        
        internal ServiceScope(ServiceProvider provider)
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

        public void AddScopedService(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            
            ThrowIfDisposed();
            
            _provider.AddDynamicService(serviceType, instance, _context);
        }

        public void PutComponentIntoService(Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            
            ThrowIfDisposed();
            _provider.PutComponentIntoService(componentType, _context);
        }

        public void StockpileComponent(MonoBehaviour component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            
            ThrowIfDisposed();
            _context.StockpileComponent(component);
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