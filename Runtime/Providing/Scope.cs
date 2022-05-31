using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace YouInject
{
    internal abstract class Scope : IScope
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
            _serviceProvider.AddService<IScope>(this);
            _logger = _serviceProvider.Resolve<IYouInjectLogger>();
            _logger.Log($"{GetType().Name} '{name}' has been created.");
        }

        public IServiceProvider ServiceProvider => _serviceProvider;
        
        public IScope CreateDerivedServiceScope(string name)
        {
            var serviceProvider = _serviceProvider.CreateDerivedServiceProvider(name);
            var derivedScope = new ServiceScope(_services, serviceProvider, name, this);
            _derivedScopes.Add(derivedScope);
            return derivedScope;
        }
        
        public virtual async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            
            while (_derivedScopes.Count > 0)
            {
                var scope = _derivedScopes.First();
                await scope.DisposeAsync();
            }

            await _serviceProvider.DisposeAsync();
            _parentScope?.RemoveDerivedScope(this);
            _logger.Log($"The {GetType().Name} '{_name}' has been disposed of.");
        }

        public static ServiceScope CreateRootScope(BakedServiceCollection services, Host host)
        {
            const string scopeName = "Root";
            var serviceProvider = YouInject.ServiceProvider.CreateRootProvider(services, scopeName);
            serviceProvider.AddService<IHost>(host);
            var scope = new ServiceScope(services, serviceProvider, scopeName, null);
            return scope;
        }

        public SceneScope CreateDerivedSceneScope(string name)
        {
            var componentProvider = _serviceProvider.CreateDerivedComponentProvider(name);
            var derivedScope = new SceneScope(_services, componentProvider, name, this);
            _derivedScopes.Add(derivedScope);
            return derivedScope;
        }

        public bool TryGetSceneScope(string sceneId, [MaybeNullWhen(false)] out SceneScope scope)
        {
            if (_derivedScopes.Count == 0)
            {
                scope = null;
                return false;
            }

            foreach (var derivedScope in _derivedScopes)
            {
                if (derivedScope.TryGetSceneScope(sceneId, out scope))
                {
                    return true;
                }
            }
            
            scope = null;
            return false;
        }

        private void RemoveDerivedScope(Scope derivedScope)
        {
            _derivedScopes.Remove(derivedScope);
        }
    }
}