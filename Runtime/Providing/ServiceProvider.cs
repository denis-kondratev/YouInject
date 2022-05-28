using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject
{
    internal class ServiceProvider : IServiceProvider
    {
        protected readonly BakedServiceCollection Services;
        protected readonly ServiceContainers Containers;
        private readonly string _scopeName;
        private readonly Stack<Type> _resolvingStack;
        private readonly IYouInjectLogger _logger;

        protected ServiceProvider(
            BakedServiceCollection services,
            string scopeName,
            ServiceContainers containers)
        {
            _scopeName = scopeName;
            Services = services;
            Containers = containers;
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

        public async ValueTask DisposeAsync()
        {
            await Containers.DisposeAsync();
        }

        internal static ServiceProvider CreateRootProvider(
            BakedServiceCollection services,
            string scopeName)
        {
            var containers = ServiceContainers.CreateRootContainers();
            var provider = new ServiceProvider(services, scopeName, containers);
            return provider;
        }

        internal ServiceProvider CreateDerivedServiceProvider(string scopeName)
        {
            var containers = Containers.CreateDerivedContainers();
            return new ServiceProvider(Services, scopeName, containers);
        }

        internal ComponentProvider CreateDerivedComponentProvider(string scopeName)
        {
            var containers = Containers.CreateDerivedComponentContainers();
            return new ComponentProvider(Services, scopeName, containers);
        }

        internal void AddService<TService>(object decision)
        {
            var serviceType = typeof(TService);
            var descriptor = Services[serviceType];
            Containers.AddDecision(decision, descriptor);
        }

        protected object MakeDecision(IServiceDescriptor descriptor)
        {
            var serviceType = descriptor.ServiceType;
            
            if (_resolvingStack.Contains(serviceType))
            {
                var anotherType = _resolvingStack.Peek();
                _resolvingStack.Clear();
                throw new Exception($"'{serviceType.Name}' and '{anotherType.Name}' services refer on each other.");
            }
            
            _resolvingStack.Push(serviceType);
            var decision = descriptor.InstantiateDecision(GetDecisions);
            
            Containers.AddDecision(decision, descriptor);
            _resolvingStack.Pop();

            var logMessage = serviceType == descriptor.DecisionType
                ? $"The service of type '{serviceType.FullName}' has been instantiated in the scope '{_scopeName}'."
                : $"The service of type '{serviceType.FullName}' has been instantiated with decision of type '{descriptor.DecisionType.FullName}' in the scope '{_scopeName}'.";
            
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            _logger?.Log(logMessage);
            
            return decision;
        }

        private object[] GetDecisions(Type[] serviceTypes)
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
            var descriptor = Services[serviceType];

            if (Containers.TryGetDecision(descriptor, out var decision))
            {
                return decision;
            }

            decision = MakeDecision(descriptor);
            return decision;
        }
    }
}