using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace YouInject
{
    internal class Host : IHost
    {
        private readonly BakedServiceCollection _services;
        private readonly ISceneLoader _sceneLoader;
        private readonly HostOptions _options;
        private readonly Dictionary<string, SceneScopeBuilder> _sceneScopeBuilders;
        private readonly ServiceScope _rootScope;

        public Host(BakedServiceCollection services, ISceneLoader sceneLoader, HostOptions options)
        {
            _services = services;
            _sceneLoader = sceneLoader;
            _options = options;
            _sceneScopeBuilders = new Dictionary<string, SceneScopeBuilder>();
            _rootScope = Scope.CreateRootScope(services, this);
        }

        public IScope RootScope => _rootScope;
        
        public async ValueTask DisposeAsync()
        {
            await _rootScope.DisposeAsync();
        }

        public SceneScopeBuilding StartBuildSceneScope(string sceneId, IScope parentScope)
        {
            if (_sceneScopeBuilders.TryGetValue(sceneId, out var builder))
            {
                return new SceneScopeBuilding(builder);
            }

            if (_rootScope.TryGetSceneScope(sceneId, out var existingScope))
            {
                return new SceneScopeBuilding(existingScope);
            }

            if (!_sceneLoader.IsSceneLoaded(sceneId))
            {
                _sceneLoader.LoadSceneAsync(sceneId);
            }

            builder = new SceneScopeBuilder(sceneId, parentScope, _services);
            _sceneScopeBuilders.Add(sceneId, builder);

            return new SceneScopeBuilding(builder);
        }
        
        public SceneScope CompleteSceneScopeBuilding(string sceneId, Component[] components)
        {
            var builder = GetSceneScopeBuilder(sceneId);
            builder.AddComponents(components);
            var scope = builder.BuildScope();
            _sceneScopeBuilders.Remove(sceneId);
            
            return scope;
        }

        private SceneScopeBuilder GetSceneScopeBuilder(string sceneId)
        {
            if (_sceneScopeBuilders.TryGetValue(sceneId, out var scope))
            {
                return scope;
            }

            throw new Exception($"Cannot find {nameof(SceneScopeBuilder)} '{sceneId}'. It has not been created or has already been used.");
        }
    }
}