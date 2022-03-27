using System;
using System.Diagnostics.CodeAnalysis;

namespace YouInject
{
    internal partial class ServiceContainers
    {
        private class DerivedContainer : Container
        {
            private readonly IServiceContainer _parentContainer;

            public DerivedContainer(IServiceContainer parentContainer)
            {
                _parentContainer = parentContainer;
            }

            public override bool TryGetDecision(Type serviceType, [NotNullWhen(true)] out object? decision)
            {
                return Decisions.TryGetValue(serviceType, out decision) 
                       || _parentContainer.TryGetDecision(serviceType, out decision);
            }
        }
    }
}