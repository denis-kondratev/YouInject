using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IServiceScope : IServiceProvider, IAsyncDisposable
    {
        void AddService(Type serviceType, object instance);
        void RemoveMonoBehaviourService(MonoBehaviour instance);
        void InitializeMonoBehaviourService(MonoBehaviour instance);
    }
}