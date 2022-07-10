using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    [CreateAssetMenu(fileName = "ScopeContext", menuName = "YouInject/Scope Context")]
    public class ScopeContext : ScriptableObject
    {
        public IServiceScope? Scope { get; private set; }

        public void CreateScope()
        {
            if (Scope is not null)
            {
                throw new InvalidOperationException("Cannot create a scope. The scope already exists.");
            }

            Scope = Host.RootScope.CreateScope();
        }

        public void DisposeOfScope()
        {
            if (Scope is null)
            {
                throw new InvalidOperationException("The scope does not exist.");
            }

            Scope.DisposeAsync();
            Scope = null;
        }
    }
}