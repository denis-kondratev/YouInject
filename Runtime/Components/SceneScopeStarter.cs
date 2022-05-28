using UnityEngine;

namespace YouInject.Components
{
    [DefaultExecutionOrder(-500)]
    public class SceneScopeStarter : MonoBehaviour
    {
        [SerializeField] private string _sceneId = null!;
        [SerializeField] private Component[] _components = null!;
        private SceneScope _scope = null!;

        private void Awake()
        {
            var builder = HostUtility.GetSceneScopeBuilder(_sceneId);
            builder.AddComponents(_components);
            _scope = HostUtility.BuildSceneScope(_sceneId);
        }

        private async void OnDestroy()
        {
            await _scope.DisposeAsync();
        }
    }
}