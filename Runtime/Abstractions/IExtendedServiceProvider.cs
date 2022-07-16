using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IExtendedServiceProvider : IServiceProvider
    {
        void AddDynamicService(Type serviceType, object instance);
        void RemoveComponent(MonoBehaviour instance);
        void InitializeComponent(MonoBehaviour instance);
        void AddComponent(MonoBehaviour instance);
    }
}