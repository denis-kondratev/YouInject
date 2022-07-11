using System;
using System.Reflection;
using UnityEngine;

namespace InjectReady.YouInject
{
    public interface IServiceScope : IServiceProvider, IAsyncDisposable
    {
        void AddService(Type serviceType, object service);
        void AddService<T>(object service);
        void RemoveService(Type serviceType);
        void InitializeService(Type serviceType, object service);
        public void InitializeService<T>(T service) where T : MonoBehaviour;
        void InitializeService(object service, MethodInfo methodInfo);
        void InitializeService(object service, string methodName);
        IServiceScope CreateScope();
    }
}