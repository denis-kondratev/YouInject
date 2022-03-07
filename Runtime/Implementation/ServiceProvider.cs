using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class ServiceProvider : IServiceProvider, IComponentInjector
    {
        private readonly ScopeSolution _solution;
        private readonly Stack<Type> _requestStack;
        private readonly Dictionary<Type, object> _injectedComponents; 

        internal ServiceProvider(ScopeSolution decisionScopeSolution)
        {
            _solution = decisionScopeSolution;
            _requestStack = new Stack<Type>();
            _injectedComponents = new Dictionary<Type, object>();
        }

        public T GetService<T>() where T : class
        {
            var serviceType = typeof(T);

            var decision = GetDecision(serviceType);

            if (!(decision is T service))
            {
                throw new Exception($"{nameof(ScopeSolution)} contains wrong decision for the '{serviceType.Name}' service.");
            }
            
            return service;
        }

        public void InjectComponent(Type serviceType, object component)
        {
            if (_solution.TryGetDecision(serviceType, out var existingDecision))
            {
                Assert.AreEqual(component, existingDecision, $"The '{serviceType.Name}' service already exists");
                return;
            }

            if (_injectedComponents.ContainsKey(serviceType))
            {
                throw new Exception($"The '{serviceType.Name}' service is already injected");
            }
            
            _injectedComponents.Add(serviceType, component);
        }
        
        public void InitializeInjectedComponents()
        {
            while (_injectedComponents.Count > 0)
            {
                var (serviceType, component) = _injectedComponents.First();
                EngageInjectedComponent(serviceType, component);
            }
        }

        public IEnumerable<(Type serviceType, Type decisionType)> GetUnimplementedServices()
        {
            var specifiers = _solution.GetUnimplementedServices();

            foreach (var specifier in specifiers)
            {
                yield return (specifier.ServiceType, specifier.DecisionType);
            }
        }

        internal object[] GetDecisions(Type[] serviceTypes)
        {
            var decisions = new object[serviceTypes.Length];

            for (var i = 0; i < serviceTypes.Length; i++)
            {
                decisions[i] = GetDecision(serviceTypes[i]);
            }
            
            return decisions;
        }

        private object GetDecision(Type serviceType)
        {
            if (_requestStack.Contains(serviceType))
            {
                var dependentType = _requestStack.Pop();
                _requestStack.Clear();
                throw new Exception($"Services {serviceType.Name} and {dependentType.Name} depend on each other.");
            }
            
            _requestStack.Push(serviceType);
            
            if (!_solution.TryGetDecision(serviceType, out var decision))
            {
                decision = MakeDecision(serviceType);
            }

            _requestStack.Pop();
            
            return decision;
        }

        private object MakeDecision(Type serviceType)
        {
            if (TryEngageInjectedComponent(serviceType, out var decision))
            {
                return decision;
            }

            var specifier = _solution.GetSpecifier(serviceType);
            
            decision = specifier.MakeDecision(this);
            _solution.AddDecision(serviceType, decision);
            
            return decision;
        }

        private bool TryEngageInjectedComponent(Type serviceType, out object decision)
        {
            if (!_injectedComponents.TryGetValue(serviceType, out decision)) return false;
            
            EngageInjectedComponent(serviceType, decision);

            return true;
        }

        private void EngageInjectedComponent(Type serviceType, object injectedComponent)
        {
            if (!(_solution.GetSpecifier(serviceType) is ComponentSpecifier specifier))
            {
                throw new Exception();
            }

            _injectedComponents.Remove(serviceType);
            specifier.InitializeComponent(this, injectedComponent);
        }
    }
}