using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace YouInject
{
    internal class BakedServiceCollection
    {
        private readonly Dictionary<Type, IServiceDescriptor> _descriptorsByDecisionType;
        private readonly Dictionary<Type, IServiceDescriptor> _descriptorsByServiceType;
        
        internal BakedServiceCollection(ServiceCollection collection)
        {
            var descriptors = collection.Descriptors;
            _descriptorsByDecisionType = descriptors.ToDictionary(descriptor => descriptor.DecisionType);
            _descriptorsByServiceType  = descriptors.ToDictionary(descriptor => descriptor.ServiceType);
        }
        
        internal bool TryGetServiceTypeByDecision(object decision, [NotNullWhen(true)]out Type? serviceType)
        {
            var decisionType = decision.GetType();

            if (_descriptorsByDecisionType.TryGetValue(decisionType, out var descriptor))
            {
                serviceType = descriptor.ServiceType;
                return true;
            }

            serviceType = null;
            return false;
        }

        internal IServiceDescriptor this[Type serviceType]
        {
            get
            {
                if (_descriptorsByServiceType.TryGetValue(serviceType, out var descriptor))
                {
                    return descriptor;
                }

                throw new Exception($"Cannot not find a descriptor for a service of the {serviceType.Name} type");
            }
        }
    }
}