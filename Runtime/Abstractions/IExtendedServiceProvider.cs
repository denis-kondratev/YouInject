using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IExtendedServiceProvider : IServiceProvider
    {
        void AddDynamicService(Type serviceType, object instance);
        void AddMonoBehaviourService(MonoBehaviour instance);
        void RemoveMonoBehaviourService(MonoBehaviour instance);
        void InitializeMonoBehaviourService(MonoBehaviour instance);
        void RememberMonoBehaviourService(MonoBehaviour instance);
    }
}