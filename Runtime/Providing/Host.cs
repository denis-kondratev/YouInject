using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject
{
    internal class Host : IHost
    {
        private readonly BakedServiceCollection _services;
        private readonly HostOptions _options;
        private readonly Dictionary<string, SceneScopeBuilder> _sceneScopeBuilders;
        private readonly ServiceScope _rootScope;

        public Host(BakedServiceCollection services, HostOptions options)
        {
            _services = services;
            _options = options;
            _sceneScopeBuilders = new Dictionary<string, SceneScopeBuilder>();
            _rootScope = Scope.CreateRootScope(services, this);
        }

        public IScope RootScope => _rootScope;
        
        public async ValueTask DisposeAsync()
        {
            await _rootScope.DisposeAsync();
        }

        public SceneScopeBuildingTask AddSceneScopeBuilder(string sceneId, IScope parentScope)
        {
            if (_sceneScopeBuilders.ContainsKey(sceneId))
            {
                throw new Exception($"Trying to add {nameof(AddSceneScopeBuilder)} '{sceneId}', but it is already exists");
            }
            
            var builder = new SceneScopeBuilder(sceneId, parentScope, _services);
            _sceneScopeBuilders.Add(sceneId, builder);
            return new SceneScopeBuildingTask(builder);
        }
        
        public ISceneScopeBuilder GetSceneScopeBuilder(string sceneId)
        {
            return GetSceneScopeBuilderPrivately(sceneId);
        }

        public SceneScope BuildSceneScope(string sceneId)
        {
            var scopeBuilder = GetSceneScopeBuilderPrivately(sceneId);
            var scope = scopeBuilder.BuildScope();
            _sceneScopeBuilders.Remove(sceneId);
            
            return scope;
        }

        private SceneScopeBuilder GetSceneScopeBuilderPrivately(string sceneId)
        {
            if (_sceneScopeBuilders.TryGetValue(sceneId, out var scope))
            {
                return scope;
            }

            throw new Exception($"Cannot find {nameof(SceneScopeBuilder)} '{sceneId}'. It has not been created or has already been used.");
        }
    }
}