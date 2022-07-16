using System;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceScope : IServiceScope, IExtendedServiceProvider
    {
        private readonly RootServiceScope _rootScope;
        private readonly ScopeContext _scopeContext;

        public IExtendedServiceProvider ServiceProvider => this;
        
        public ServiceScope(RootServiceScope rootScope, ScopeContext scopeContext)
        {
            _rootScope = rootScope;
            _scopeContext = scopeContext;
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
        
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public void AddDynamicService(Type serviceType, object instance)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        public void InitializeComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        public void AddComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }
    }
}