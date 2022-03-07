using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class ServiceContainer : IDisposable
    {
        private readonly Dictionary<Type, object> _decisions;
        private bool _isDisposed;

        internal ServiceContainer()
        {
            _decisions = new Dictionary<Type, object>();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            
            foreach (var decision in _decisions.Values)
            {
                if (decision is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        internal void AddDecision(Type serviceType, object decision)
        {
            AssertItIsNotDisposed();
            
            Assert.IsNotNull(serviceType, $"{nameof(serviceType)} cannot be null.");
            Assert.IsNotNull(decision, $"{nameof(decision)} cannot be null.");
            Assert.IsFalse(_decisions.ContainsKey(serviceType), $"The {nameof(ServiceContainer)} already contains '{serviceType.Name}' service.");
            
            _decisions.Add(serviceType, decision);
        }

        internal virtual bool TryGetDecision(Type serviceType, out object decision)
        {
            AssertItIsNotDisposed();

            return _decisions.TryGetValue(serviceType, out decision);
        }

        internal virtual bool Contains(Type serviceType)
        {
            var hasDecision = _decisions.ContainsKey(serviceType);

            return hasDecision;
        }
        
        protected void AssertItIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"{nameof(ServiceContainer)} is disposed.");
            }
        }
    }
}