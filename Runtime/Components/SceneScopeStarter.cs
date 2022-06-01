using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace YouInject.Components
{
    [DefaultExecutionOrder(-500)]
    public class SceneScopeStarter : MonoBehaviour
    {
        [SerializeField] private string _sceneId = null!;
        [SerializeField] private Component[] _components = null!;
#if UNITY_EDITOR
        [SerializeField] private bool canStartDelayBelatedly;
#endif
        
        private IScope? _scope;
        
        private void Awake()
        {
            var scopeProvider = HostUtility.GetSceneScopeProvider();
            
#if UNITY_EDITOR
            if (!canStartDelayBelatedly)
            {
                _scope = scopeProvider.InitializeSceneScope(_sceneId, _components);
                return;
            }

            if (scopeProvider.TryInitializeSceneScope(_sceneId, _components, out _scope))
            {
                return;
            }
            
            InitializeScopeBelatedly(scopeProvider);
#else
            _scope = scopeProvider.InitializeSceneScope(_sceneId, _components);
#endif
        }

        private void InitializeScopeBelatedly(ISceneScopeProvider scopeProvider)
        {
            var buffer = DisableComponents();
            StartCoroutine(DoAsync());

            IEnumerator DoAsync()
            {
                yield return scopeProvider.WaitUntilScopeBuilderAdded(_sceneId);
                _scope = scopeProvider.InitializeSceneScope(_sceneId, _components);
                RestoreComponents(buffer);
            }
        }

        private bool[] DisableComponents()
        {
            var buffer = new bool[_components.Length];
            
            for (var i = 0; i < _components.Length; i++)
            {
                if (_components[i] is not MonoBehaviour monoBehaviour) continue;
                buffer[i] = monoBehaviour.gameObject.activeSelf;
                monoBehaviour.gameObject.SetActive(false);
            }

            return buffer;
        }

        private void RestoreComponents(bool[] buffer)
        {
            Assert.AreEqual(_components.Length, buffer.Length);
            
            for (var i = 0; i < _components.Length; i++)
            {
                if (_components[i] is not MonoBehaviour monoBehaviour) continue;
                monoBehaviour.gameObject.SetActive(buffer[i]);
            }
        }

        private void OnDestroy()
        {
            _scope?.DisposeAsync();
        }
    }
}