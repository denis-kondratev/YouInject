using System;

namespace YouInject
{
    internal class ChainServiceContainer : ServiceContainer
    {
        private readonly ChainServiceContainer? _parentContainer;

        internal ChainServiceContainer() { }
        
        private ChainServiceContainer(ChainServiceContainer? parentContainer)
        {
            _parentContainer = parentContainer;
        }

        internal ChainServiceContainer CreateContainer()
        {
            return new ChainServiceContainer(this);
        }

        internal override bool TryGetDecision(Type serviceType, out object decision)
        {
            if (base.TryGetDecision(serviceType, out decision))
            {
                return true;
            }

            return _parentContainer?.TryGetDecision(serviceType, out decision) ?? false;
        }

        internal override bool Contains(Type serviceType)
        {
            if (base.Contains(serviceType))
            {
                return true;
            }

            return _parentContainer?.Contains(serviceType) ?? false;
        }
    }
}