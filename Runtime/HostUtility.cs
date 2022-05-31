using System;
using UnityEngine;

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
        
        public static IScope CompleteSceneScopeBuilding(string sceneId, Component[] components)
        {
            if (_host is null)
            {
                throw new Exception($"Trying to build scene scope '{sceneId}', but the host doesn't exist.");
            }
            
            var scope = _host.CompleteSceneScopeBuilding(sceneId, components);
            return scope;
        }
    }
}