using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class ScopeSolution : IDisposable
    {
        private readonly ServiceCollection _collection;
        private readonly ChainServiceContainer _persistents;
        private readonly ServiceContainer _transients;
        private readonly ServiceContainer _singletons;
        private bool _isDisposed;
        
        internal ScopeSolution(ServiceCollection serviceCollection, ServiceContainer singletons)
        {
            _collection = serviceCollection;
            _persistents = new ChainServiceContainer();
            _transients = new ServiceContainer();
            _singletons = singletons;
        }

        private ScopeSolution(ScopeSolution baseProvider)
        {
            _transients = new ServiceContainer();
            _collection = baseProvider._collection;
            _singletons = baseProvider._singletons;
            _persistents = baseProvider._persistents.CreateContainer();
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            _persistents.Dispose();
            _transients.Dispose();
        }

        internal ScopeSolution CreateSolution()
        {
            var descendant = new ScopeSolution(this);

            return descendant;
        }

        internal bool HasService(Type serviceType)
        {
            AssertItIsNotDisposed();
            
            var specifier = _collection.GetSpecifier(serviceType);
            var container = GetContainer(specifier.Lifetime);
            var result = container.Contains(serviceType);

            return result;
        }

        internal ServiceSpecifier GetSpecifier(Type serviceType)
        {
            AssertItIsNotDisposed();
            
            var specifier = _collection.GetSpecifier(serviceType);

            return specifier;
        }
        
        internal bool TryGetDecision(Type serviceType, [MaybeNullWhen(false)]out object decision)
        {
            AssertItIsNotDisposed();
            
            var specifier = _collection.GetSpecifier(serviceType);

            if (specifier.Lifetime == ServiceLifetime.Transient)
            {
                decision = null;
                return false;
            }
            
            var container = GetContainer(specifier.Lifetime);
            var hasDecision = container.TryGetDecision(serviceType, out decision);

            return hasDecision;
        }
        
        internal IEnumerable<ServiceSpecifier> GetUnimplementedServices()
        {
            var specifiers = _collection.GetSpecifiers();

            foreach (var specifier in specifiers)
            {
                if (!(specifier is ComponentSpecifier) || specifier.Lifetime == ServiceLifetime.Transient)
                {
                    continue;
                }

                var container = GetContainer(specifier.Lifetime);
                
                if (container.Contains(specifier.ServiceType)) continue;

                yield return specifier;
            }
        }
        
        private ServiceContainer GetContainer(ServiceLifetime serviceLifetime)
        {
            var container = serviceLifetime switch
            {
                ServiceLifetime.Singleton => _singletons,
                ServiceLifetime.Scoped => _persistents,
                ServiceLifetime.Transient => _transients,
                _ => throw new ArgumentOutOfRangeException()
            };

            return container;
        }
        
        private void AssertItIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"{nameof(ScopeSolution)} is disposed.");
            }
        }

        public void AddDecision(Type serviceType, object decision)
        {
            var specifier = _collection.GetSpecifier(serviceType);
            
            Assert.AreEqual(decision.GetType(), specifier.DecisionType, "The being added decision has wrong type");
            
            var container = GetContainer(specifier.Lifetime);
            
            container.AddDecision(serviceType, decision);
        }
    }
}