using System;
using System.Linq;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class ServiceDescriptor : IServiceDescriptor
    {
        private readonly Type[] _parameterTypes;
        public Type ServiceType { get; }
        public Type DecisionType { get; }
        public ServiceLifetime Lifetime { get; }

        internal ServiceDescriptor(Type serviceType, Type decisionType, ServiceLifetime lifetime)
        {
            Assert.IsFalse(decisionType.IsSubclassOf(ComponentDescriptor.ComponentType));
            
            ServiceType = serviceType;
            DecisionType = decisionType;
            Lifetime = lifetime;
            _parameterTypes = GetParameterTypes(decisionType);
        }

        internal object MakeDecision(ServiceProvider serviceProvider)
        {
            var parameters = new object[_parameterTypes.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = serviceProvider.GetDecision(_parameterTypes[i]);
            }
            
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