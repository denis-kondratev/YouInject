using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IExtendedServiceProvider : IServiceProvider
    {
        void PutComponentIntoService(Type componentType);
        void StockpileComponent(MonoBehaviour component);
    }
}