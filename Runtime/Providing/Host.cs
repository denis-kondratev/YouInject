using System;
using System.Collections.Generic;

namespace YouInject
{
    internal class Host : IHost
    {
        private readonly BakedServiceCollection _services;
        private readonly Dictionary<string, SceneScopeBuilder> _sceneScopeBuilders;
        private readonly Dictionary<string, Scope> _sceneScopes;
        private readonly Scope _rootScope;
        private readonly Scope _rootSceneScope;

        public Host(BakedServiceCollection services)
        {
            _services = services;
            _rootScope = Scope.CreateRootScope(services);
            _rootSceneScope = _rootScope;
            _sceneScopeBuilders = new Dictionary<string, SceneScopeBuilder>();
            _sceneScopes = new Dictionary<string, Scope>();
            Instance = this;
        }

        public IScope RootScope => _rootScope;
        internal static Host Instance { get; private set; } = null!;

        public void Dispose()
        {
            _rootScope.Dispose();
        }

        internal SceneScopeBuilder GetSceneScopeBuilder(string scenePath)
        {
            if (_sceneScopeBuilders.TryGetValue(scenePath, out var scope)) return scope;
            
            scope = new SceneScopeBuilder(_services);
            _sceneScopeBuilders.Add(scenePath, scope);
            return scope;
        }

        internal void BuildSceneScope(string scenePath, string parentScenePath)
        {
            var scopeBuilder = GetSceneScopeBuilder(scenePath);
            
            if (!_sceneScopes.TryGetValue(parentScenePath, out var parentScope) 
                && !string.IsNullOrWhiteSpace(parentScenePath))
            {
                throw new Exception($"Failed building a scope of the '{scenePath}' scene because cannot find " +
                                    $"parent scope of the '{parentScenePath}' scene.");
            }

            parentScope ??= _rootSceneScope;
            var scope = scopeBuilder.BuildScope(parentScope, scenePath);
            _sceneScopes.Add(scenePath, scope);
        }

        internal void DisposeOfSceneScope(string scenePath)
        {
            if (_sceneScopes.Remove(scenePath, out var scope))
            {
                scope.Dispose();
            }
        }
    }
}