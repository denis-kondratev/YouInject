using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YouInject
{
    internal class Scope : IScope
    {
        private readonly Scope? _parentScope;
        private readonly string _name;
        private readonly HashSet<Scope> _derivedScopes;
        private readonly BakedServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;
        private readonly IYouInjectLogger _logger;
        private bool _isDisposed;

        private Scope(BakedServiceCollection services)
        {
            const string name = "Root";
            
            _services = services;
            _serviceProvider = YouInject.ServiceProvider.CreateRootProvider(services, name);
            _derivedScopes = new HashSet<Scope>();
            _name = name;
            _logger = _serviceProvider.Resolve<IYouInjectLogger>();
            _logger.Log("Scope was created: " + name);
        }

        private Scope(Scope parentScope, string name)
        {
            _services = parentScope._services;
            _serviceProvider = parentScope._serviceProvider.CreateDerivedProvider(name);
            _derivedScopes = new HashSet<Scope>();
            _parentScope = parentScope;
            _name = name;
            _logger = _serviceProvider.Resolve<IYouInjectLogger>();
            _logger.Log($"The scope '{name}' has been created.");
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

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
            
            _logger.Log($"The scope '{_name}' has been disposed of.");
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

        internal Scope CreateDerivedScope(string name)
        {
            var derivedScope = new Scope(this, name);
            _derivedScopes.Add(derivedScope);
            return derivedScope;
        }

        private void RemoveDerivedScope(Scope derivedScope)
        {
            _derivedScopes.Remove(derivedScope);
        }
    }
}