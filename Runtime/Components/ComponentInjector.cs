using UnityEngine;

namespace YouInject.Components
{
    public class ComponentInjector : MonoBehaviour
    {
#nullable disable
        [SerializeField] private Component _component;
#nullable restore

        private void Awake()
        {
            var scopeBuilder = Host.Instance.GetSceneScopeBuilder(gameObject.scene.path);
            scopeBuilder.InjectComponent(_component);
        }
    }
}