using UnityEngine;

namespace YouInject.Components
{
    public class SceneScope : MonoBehaviour
    {
        [SerializeField] private string _parentScenePath = null!;

        private void Awake()
        {
            var currentScenePath = gameObject.scene.path;
            HostUtility.BuildSceneScope(currentScenePath, _parentScenePath);
        }

        private void OnDestroy()
        {
            HostUtility.DisposeOfSceneScope(gameObject.scene.path);
        }
    }
}