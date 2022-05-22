using UnityEngine;

namespace YouInject.Components
{
    public class SceneScope : MonoBehaviour
    {
        [SerializeField] private string _parentScenePath = null!;

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