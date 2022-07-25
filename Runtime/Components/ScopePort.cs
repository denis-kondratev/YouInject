using InjectReady.YouInject.Internal;
using UnityEngine;

namespace InjectReady.YouInject
{
    public class ScopePort : ScriptableObject
    {
        private IServiceScope? _scope;

        public IServiceScope Scope
        {
            get
            {
                if (_scope is null) throw ExceptionBuilder.ScopeDoesNotExist();
                
                return _scope;
            }
        }

        public void CreateScope()
        {
            if (_scope is not null) throw ExceptionBuilder.ScopeAlreadyExists();
                
            _scope = Host.ServiceProvider.CreateScope();
        }

        public void DisposeOfScope()
        {
            if (_scope is null) throw ExceptionBuilder.ScopeDoesNotExist();

            var scope = _scope;
            _scope = null;
            scope.DisposeAsync();
        }
    }
}