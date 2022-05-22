using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YouInject
{
    internal class Scope : IScope
    {
        private readonly Scope? _parentScope;
        private readonly HashSet<Scope> _derivedScopes;
        private readonly BakedServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;
        private bool _isDisposed;

        public IServiceProvider ServiceProvider => _serviceProvider;

        private Scope(BakedServiceCollection services)
        {
            _services = services;
            _serviceProvider = YouInject.ServiceProvider.CreateRootProvider(services);
            _derivedScopes = new HashSet<Scope>();
        }

        private Scope(Scope parentScope) : this (parentScope._services)
        {
            _services = parentScope._services;
            _serviceProvider = parentScope._serviceProvider.CreateDerivedProvider();
            _derivedScopes = new HashSet<Scope>();
            _parentScope = parentScope;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            
            while (_derivedScopes.Count > 0)
            {
                var scope = _derivedScopes.First();
                scope.Dispose();
            }

            _parentScope?.RemoveDerivedScope(this);
        }

        internal static Scope CreateRootScope(BakedServiceCollection services)
        {
            var scope = new Scope(services);
            return scope;
        }
        
        internal void AddComponents(Dictionary<Type, Component> components)
        {
            _serviceProvider.AddComponents(components);
        }

        internal Scope CreateDerivedScope()
        {
            var derivedScope = new Scope(this);
            _derivedScopes.Add(derivedScope);
            return derivedScope;
        }

        private void RemoveDerivedScope(Scope derivedScope)
        {
            _derivedScopes.Remove(derivedScope);
        }
    }
}