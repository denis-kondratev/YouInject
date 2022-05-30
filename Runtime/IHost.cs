using System;

namespace YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IScope RootScope { get; }
        SceneScopeBuildingTask AddSceneScopeBuilder(string sceneId, IScope parentScope);
    }
}