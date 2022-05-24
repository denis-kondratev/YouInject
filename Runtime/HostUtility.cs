using System;

namespace YouInject
{
    public static partial class HostUtility
    {
        private static Host? _host;
        
        public static IHostBuilder CreateBuilder()
        {
            var builder = new HostBuilder();
            return builder;
        }

        public static void DestroyHost()
        {
            _host?.Dispose();
            _host = null;
        }

        internal static void BuildSceneScope(string scenePath, string parentSceneScope)
        {
            if (_host is null)
            {
                throw new Exception($"Trying to build scene scope '{scenePath}', but the host doesn't exist.");
            }
            
            _host.BuildSceneScope(scenePath, parentSceneScope);
        }

        internal static void DisposeOfSceneScope(string scenePath)
        {
            _host?.DisposeOfSceneScope(scenePath);
        }

        internal static SceneScopeBuilder GetSceneScopeBuilder(string scenePath)
        {
            if (_host is null)
            {
                throw new Exception($"Trying to get scene scope builder {scenePath}, but the host doesn't exist.");
            }
            
            return _host.GetSceneScopeBuilder(scenePath);
        }
    }
}