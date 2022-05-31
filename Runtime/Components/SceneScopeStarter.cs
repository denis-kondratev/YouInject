using UnityEngine;

namespace YouInject.Components
{
    [DefaultExecutionOrder(-500)]
    public class SceneScopeStarter : MonoBehaviour
    {
        [SerializeField] private string _sceneId = null!;
        [SerializeField] private Component[] _components = null!;
        private IScope _scope = null!;

        private void Awake()
        {
            _scope = HostUtility.CompleteSceneScopeBuilding(_sceneId, _components);
        }

        private async void OnDestroy()
        {
            await _scope.DisposeAsync();
        }
    }
}