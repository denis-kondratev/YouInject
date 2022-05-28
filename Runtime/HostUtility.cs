using System;

namespace YouInject
{
    public static partial class HostUtility
    {
        private static Host? _host;
        
        public static IHostBuilder CreateHostBuilder()
        {
            var builder = new HostBuilder();
            return builder;
        }
        
        internal static SceneScope BuildSceneScope(string sceneId)
        {
            if (_host is null)
            {
                throw new Exception($"Trying to build scene scope '{sceneId}', but the host doesn't exist.");
            }
            
            var scope = _host.BuildSceneScope(sceneId);
            return scope;
        }
        
        internal static ISceneScopeBuilder GetSceneScopeBuilder(string scenePath)
        {
            if (_host is null)
            {
                throw new Exception($"Trying to get scene scope builder {scenePath}, but the host doesn't exist.");
            }
            
            return _host.GetSceneScopeBuilder(scenePath);
        }
    }
}