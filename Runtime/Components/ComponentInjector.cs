using UnityEngine;

namespace YouInject.Components
{
    [DefaultExecutionOrder(-510)]
    public class ComponentInjector : MonoBehaviour
    {
        [SerializeField] private Component _component = null!;
        
        private void Awake()
        {
            var scopeBuilder = HostUtility.GetSceneScopeBuilder(gameObject.scene.path);
            scopeBuilder.InjectComponent(_component);
        }
    }
}