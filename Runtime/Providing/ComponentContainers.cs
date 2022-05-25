using UnityEngine.Assertions;

namespace YouInject
{
    internal class ComponentContainers : ServiceContainers
    {
        private readonly ServiceContainers _parentContainers;

        internal ComponentContainers(ServiceContainers parentContainers) : base(parentContainers)
        {
            _parentContainers = parentContainers;
        }
        
        internal override void AddDecision(object decision, IServiceDescriptor descriptor)
        {
            Assert.IsNotNull(decision);

            if (descriptor is ComponentDescriptor)
            {
                base.AddDecision(decision, descriptor);
                return;
            }
            
            _parentContainers.AddDecision(decision, descriptor);
        }
    }
}