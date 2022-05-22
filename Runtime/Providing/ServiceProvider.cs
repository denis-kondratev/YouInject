using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YouInject
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly BakedServiceCollection _services;
        private readonly string _scopeName;
        private readonly ServiceContainers _containers;
        private readonly Stack<Type> _resolvingStack;
        private readonly IYouInjectLogger _logger;

        private ServiceProvider(BakedServiceCollection services, string scopeName)
        {
            _services = services;
            _scopeName = scopeName;
            _containers = ServiceContainers.CreateRootContainers();
            _resolvingStack = new Stack<Type>();
            _logger = Resolve<IYouInjectLogger>();
        }
        
        private ServiceProvider(ServiceProvider parentProvider, string scopeName)
        {
            _scopeName = scopeName;
            _services = parentProvider._services;
            _containers = parentProvider._containers.CreateDerivedContainers();
            _resolvingStack = new Stack<Type>();
            _logger = Resolve<IYouInjectLogger>();
        }

        public TService Resolve<TService>()
        {
            var serviceType = typeof(TService);
            var decision = GetDecision(serviceType);

            if (decision is TService service)
            {
                return service;
            }

            throw new Exception($"Cannot resolve '{typeof(TService).Name}' service. Decision type '{decision.GetType().Name}' is not derived from the service one.");
        }
        
        internal static ServiceProvider CreateRootProvider(BakedServiceCollection services, string scopeName)
        {
            var provider = new ServiceProvider(services, scopeName);
            return provider;
        }

        internal ServiceProvider CreateDerivedProvider(string scopeName)
        {
            return new ServiceProvider(this, scopeName);
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

            var logMessage = serviceType == descriptor.DecisionType
                ? $"The service of type '{serviceType.FullName}' has been instantiated in the scope '{_scopeName}'."
                : $"The service of type '{serviceType.FullName}' has been instantiated with decision of type '{descriptor.DecisionType.FullName}' in the scope '{_scopeName}'.";
            
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            _logger?.Log(logMessage);
            
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