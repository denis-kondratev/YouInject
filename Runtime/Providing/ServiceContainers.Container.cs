using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ServiceContainers
    {
        private class Container : IServiceContainer
        {
            protected readonly Dictionary<Type, object> Decisions;
            private bool _isDisposed;

            public Container()
            {
                Decisions = new Dictionary<Type, object>();
            }

            public void AddDecision(object decision, Type serviceType)
            {
                Assert.IsFalse(_isDisposed);

                Decisions.Add(serviceType, decision);
            }

            public virtual bool TryGetDecision(Type serviceType, [NotNullWhen(true)] out object? decision)
            {
                Assert.IsFalse(_isDisposed);

                return Decisions.TryGetValue(serviceType, out decision);
            }

            public IServiceContainer CreateDerivedContainer()
            {
                return new DerivedContainer(this);
            }

            public void Dispose()
            {
                if (_isDisposed) return;

                _isDisposed = true;
                
                foreach (var decision in Decisions.Values)
                {
                    if (decision is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                
                Decisions.Clear();
            }
        }
    }
}