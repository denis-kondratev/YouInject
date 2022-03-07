using System.Collections.Generic;

namespace YouInject
{
    internal class Scope : IScope
    {
        private readonly  ServiceCollection _collection;
        private bool _isDisposed;
        private readonly List<Scope> _subScopes;
        private readonly Scope? _parentScope;
        private readonly ScopeSolution _scopeSolution;

        internal Scope(ServiceCollection collection, ServiceContainer singletons)
        {
            _collection = collection;
            _scopeSolution = new ScopeSolution(collection, singletons);
            _subScopes = new List<Scope>();
        }

        private Scope(Scope parentScope)
        {
            _parentScope = parentScope;
            _collection = parentScope._collection;
            _scopeSolution = parentScope._scopeSolution.CreateSolution();
            _subScopes = new List<Scope>();
        }

        public IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new ServiceProvider(_scopeSolution);

            return serviceProvider;
        }

        public IComponentInjector GetComponentInjector()
        {
            var serviceProvider = new ServiceProvider(_scopeSolution);

            return serviceProvider;
        }

        public IScope CreateScope()
        {
            var scope = new Scope(this);
            _subScopes.Add(scope);

            return scope;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            _isDisposed = true;

            foreach (var subScope in _subScopes)
            {
                subScope.Dispose();
            }

            if (!_parentScope?._isDisposed ?? false)
            {
                _parentScope?.RemoveScope(this);
            }
        }

        private void RemoveScope(Scope scope)
        {
            _subScopes.Remove(scope);
        }
    }
}