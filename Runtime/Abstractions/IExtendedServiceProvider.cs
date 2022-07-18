using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IExtendedServiceProvider : IServiceProvider
    {
        void AddDynamicService(Type serviceType, object instance);
        void PutComponentIntoService(Type componentType);
        void StockpileComponent(MonoBehaviour component);
    }
}