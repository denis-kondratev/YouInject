using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IExtendedServiceProvider : IServiceProvider
    {
        void AddDynamicService(Type serviceType, object instance);
        void InitializeComponent(MonoBehaviour instance);
        void StockpileComponent(MonoBehaviour component);
    }
}