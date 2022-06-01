using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace YouInject
{
    public interface ISceneScopeProvider
    {
        IScope InitializeSceneScope(string sceneId, Component[] components);
        bool TryInitializeSceneScope(string sceneId, Component[] components, [MaybeNullWhen(false)] out IScope scope);
        IEnumerator WaitUntilScopeBuilderAdded(string sceneId);
    }
}