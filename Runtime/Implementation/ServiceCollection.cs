using System;
using System.Collections.Generic;

namespace YouInject
{
    internal class ServiceCollection
    {
        private readonly Dictionary<Type, ServiceSpecifier> _specifiers;

        internal ServiceCollection(List<ServiceRegistration> configurations)
        {
            _specifiers = new Dictionary<Type, ServiceSpecifier>(configurations.Count);
            
            foreach (var registration in configurations)
            {
                var specifier = registration.BuildSpecifier();
                _specifiers.Add(specifier.ServiceType, specifier);
            }
        }

        internal ServiceSpecifier GetSpecifier(Type serviceType)
        {
            if (_specifiers.TryGetValue(serviceType, out var serviceInfo))
            {
                return serviceInfo;
            }

            throw new Exception($"The Service Descriptor doesn't contain '{serviceType.Name}' service.");
        }

        internal IEnumerable<ServiceSpecifier> GetSpecifiers()
        {
            foreach (var specifierEntry in _specifiers)
            {
                yield return specifierEntry.Value;
            }
        }
    }
}