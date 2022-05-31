using System;

namespace YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IScope RootScope { get; }
        SceneScopeBuilding StartBuildSceneScope(string sceneId, IScope parentScope);
    }
}