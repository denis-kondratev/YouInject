using UnityEngine;

namespace InjectReady.YouInject
{
    [DefaultExecutionOrder(-1000)]
    public class ComponentInjector : MonoBehaviour
    {
        [SerializeField] private ScopePort _scopePort = null!;
        [SerializeField] private MonoBehaviour _component = null!;

        private void Awake()
        {
            _scopePort.Scope.StockpileComponent(_component);
        }
    }
}