using UnityEngine;

namespace YouInject.Components
{
    public class SceneScope : MonoBehaviour
    {
#nullable disable
        [SerializeField] private string _parentScenePath;
#nullable restore

        private void Awake()
        {
            var currentScenePath = gameObject.scene.path;
            Host.Instance.BuildSceneScope(currentScenePath, _parentScenePath);
        }

        private void OnDestroy()
        {
            Host.Instance.DisposeOfSceneScope(gameObject.scene.path);
        }
    }
}