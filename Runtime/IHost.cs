using System;

namespace YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IScope RootScope { get; }
        void AddSceneScopeBuilder(string sceneId, IScope parentScope);
    }
}