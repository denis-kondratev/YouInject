using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YouInject
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly BakedServiceCollection _services;
        private readonly ServiceContainers _containers;
        private readonly Stack<Type> _resolvingStack;

        private ServiceProvider(BakedServiceCollection services)
        {
            _services = services;
            _containers = ServiceContainers.CreateRootContainers();
            _resolvingStack = new Stack<Type>();
        }
        
        private ServiceProvider(ServiceProvider parentProvider)
        {
            _services = parentProvider._services;
            _containers = parentProvider._containers.CreateDerivedContainers();
            _resolvingStack = new Stack<Type>();
        }

        internal static ServiceProvider CreateRootProvider(BakedServiceCollection services)
        {
            var provider = new ServiceProvider(services);
            return provider;
        }

        internal ServiceProvider CreateDerivedProvider()
        {
            return new ServiceProvider(this);
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

        internal object[] GetDecisions(Type[] serviceTypes)
        {
            var decisions = new object[serviceTypes.Length];

            for (var i = 0; i < decisions.Length; i++)
            {
                decisions[i] = GetDecision(serviceTypes[i]);
            }

            return decisions;
        }

        private object GetDecision(Type serviceType)
        {
            var descriptor = _services[serviceType];

            if (_containers.TryGetDecision(descriptor, out var decision))
            {
                return decision;
            }

            decision = MakeDecision(descriptor);
            return decision;
        }

        private object MakeDecision(IServiceDescriptor descriptor)
        {
            var serviceType = descriptor.ServiceType;
            
            if (_resolvingStack.Contains(serviceType))
            {
                var anotherType = _resolvingStack.Peek();
                _resolvingStack.Clear();
                throw new Exception($"'{serviceType.Name}' and '{anotherType.Name}' services refer on each other.");
            }
            
            _resolvingStack.Push(serviceType);
            var decision = descriptor.InstantiateDecision(this);
            _containers.AddDecision(decision, descriptor);
            _resolvingStack.Pop();
            return decision;
        }

        private void AddComponentAccountingTheRest(Type serviceType, Component component, Dictionary<Type, Component> restComponents)
        {
            if (_services[serviceType] is not ComponentDescriptor descriptor)
            {
                throw new Exception($"Cannot find a {nameof(ComponentDescriptor)} for the '{serviceType.Name}' service.");
            }

            if (descriptor.InitializingParameterTypes is not null)
            {
                var parameters = GetDecisionsAccountingComponents(descriptor.InitializingParameterTypes, restComponents);
                descriptor.InitializeComponent(component, parameters);
            }
            
            _containers.AddDecision(component, descriptor);
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
                var descriptor = _services[serviceType];

                if (_containers.TryGetDecision(descriptor, out var decision))
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