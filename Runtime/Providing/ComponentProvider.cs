using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YouInject
{
    internal class ComponentProvider : ServiceProvider
    {
        internal ComponentProvider(
            BakedServiceCollection services,
            string scopeName,
            ServiceContainers containers) 
            : base(services, scopeName, containers)
        {
        }
        
        internal void AddComponents(Dictionary<Type, Component> components)
        {
            while (components.Count > 0)
            {
                var (serviceType, component) = components.First();
                components.Remove(serviceType);
                AddComponentAccountingTheRest(serviceType, component, components);
            }
        }
        
        private void AddComponentAccountingTheRest(Type serviceType, Component component, Dictionary<Type, Component> restComponents)
        {
            if (Services[serviceType] is not ComponentDescriptor descriptor)
            {
                throw new Exception($"Cannot find a {nameof(ComponentDescriptor)} for the '{serviceType.Name}' service.");
            }

            if (descriptor.InitializingParameterTypes is not null)
            {
                var parameters = GetDecisionsAccountingComponents(descriptor.InitializingParameterTypes, restComponents);
                descriptor.InitializeComponent(component, parameters);
            }
            
            Containers.AddDecision(component, descriptor);
        }
        
        private object[] GetDecisionsAccountingComponents(
            IReadOnlyList<Type> servicesTypes,
            Dictionary<Type, Component> restComponents)
        {
            if (servicesTypes.Count == 0) return Array.Empty<object>();

            var decisions = new object[servicesTypes.Count];
            
            for (var i = 0; i < decisions.Length; i++)
            {
                var serviceType = servicesTypes[i];
                var descriptor = Services[serviceType];

                if (Containers.TryGetDecision(descriptor, out var decision))
                {
                    decisions[i] = decision;
                    continue;
                }

                if (restComponents.TryGetValue(serviceType, out var component))
                {
                    restComponents.Remove(serviceType);
                    AddComponentAccountingTheRest(serviceType, component, restComponents);
                    decisions[i] = component;
                    continue;
                }

                decisions[i] = MakeDecision(descriptor);
            }

            return decisions;
        }
    }
}