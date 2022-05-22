using System;
using System.Linq;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class ServiceDescriptor : IRawServiceDescriptor, IServiceDescriptor
    {
        private bool _isBaked;
        private readonly Type[] _parameterTypes;
        public Type ServiceType { get; }
        public Type DecisionType { get; }
        public ServiceLifetime Lifetime { get; }

        internal ServiceDescriptor(Type serviceType, Type decisionType, ServiceLifetime lifetime)
        {
            Assert.IsFalse(decisionType.IsSubclassOf(ComponentDescriptor.ComponentType));
            Assert.IsTrue(serviceType.IsAssignableFrom(decisionType));
            
            ServiceType = serviceType;
            DecisionType = decisionType;
            Lifetime = lifetime;
            _parameterTypes = GetParameterTypes(decisionType);
        }

        public IServiceDescriptor Bake()
        {
            Assert.IsFalse(_isBaked);
            
            _isBaked = true;
            return this;
        }

        public object InstantiateDecision(ServiceProvider serviceProvider)
        {
            Assert.IsTrue(_isBaked);

            var parameters = serviceProvider.GetDecisions(_parameterTypes);
            var decision = Activator.CreateInstance(DecisionType, parameters);
            return decision;
        }
        
        private static Type[] GetParameterTypes(Type decisionType)
        {
            var constructors = decisionType.GetConstructors();

            if (constructors.Length == 0) return Array.Empty<Type>();

            var parameters = constructors[0].GetParameters();
            
            for (var i = 1; i < constructors.Length; i++)
            {
                var applicantParameters = constructors[i].GetParameters();

                if (applicantParameters.Length > parameters.Length)
                {
                    parameters = applicantParameters;
                }
            }

            var result = parameters.Select(parameter => parameter.ParameterType).ToArray();
            return result;
        }
    }
}