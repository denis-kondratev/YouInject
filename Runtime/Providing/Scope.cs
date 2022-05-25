using System.Collections.Generic;
using System.Linq;

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

        protected Scope(BakedServiceCollection services, ServiceProvider serviceProvider, string name, Scope? parentScope)
        {
            _services = services;
            _serviceProvider = serviceProvider;
            _derivedScopes = new HashSet<Scope>();
            _parentScope = parentScope;
            _name = name;
            _logger = _serviceProvider.Resolve<IYouInjectLogger>();
            _logger.Log($"{GetType().Name} '{name}' has been created.");
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
            
            _logger.Log($"The scope '{_name}' has been disposed of.");
        }

        internal static Scope CreateRootScope(BakedServiceCollection services)
        {
            const string scopeName = "Root";
            var serviceProvider = ServiceProvider.CreateRootProvider(services, scopeName);
            var scope = new Scope(services, serviceProvider, scopeName, null);
            return scope;
        }

        internal Scope CreateDerivedScope(string name)
        {
            var serviceProvider = _serviceProvider.CreateDerivedProvider(name);
            var derivedScope = new Scope(_services, serviceProvider, name, this);
            _derivedScopes.Add(derivedScope);
            return derivedScope;
        }
        
        internal SceneScope CreateDerivedSceneScope(string name)
        {
            var componentProvider = _serviceProvider.CreateDerivedComponentProvider(name);
            var derivedScope = new SceneScope(_services, componentProvider, name, this);
            _derivedScopes.Add(derivedScope);
            return derivedScope;
        }

        private void RemoveDerivedScope(Scope derivedScope)
        {
            _derivedScopes.Remove(derivedScope);
        }
    }
}