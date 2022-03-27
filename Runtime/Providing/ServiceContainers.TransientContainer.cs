using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ServiceContainers
    {
        private class TransientContainer : IServiceContainer
        {
            private readonly List<object> _decisions;
            private bool _isDisposed;
            
            public TransientContainer()
            {
                _decisions = new List<object>();
            }

            public void AddDecision(object decision, Type _)
            {
                Assert.IsFalse(_isDisposed);

                _decisions.Add(decision);
            }

            public bool TryGetDecision(Type serviceType, [NotNullWhen(true)] out object? decision)
            {
                Assert.IsFalse(_isDisposed);

                decision = null;
                return false;
            }

            public IServiceContainer CreateDerivedContainer()
            {
                return new DerivedContainer(this);
            }

            public void Dispose()
            {
                if (_isDisposed) return;

                _isDisposed = true;
                
                foreach (var decision in _decisions)
                {
                    if (decision is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                
                _decisions.Clear();
            }
        }
    }
}