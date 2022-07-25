using UnityEngine;

namespace InjectReady.YouInject
{
    [DefaultExecutionOrder(-900)]
    public class ComponentInitializer : MonoBehaviour
    {
        [SerializeField] private ScopePort _scopePort = null!;
        [SerializeField] private MonoBehaviour _component = null!;

        private void Awake()
        {
            _scopePort.Scope.PutComponentIntoService(_component.GetType());
        }
    }
}