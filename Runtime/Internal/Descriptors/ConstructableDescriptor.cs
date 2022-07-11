using System;
using System.Reflection;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ConstructableDescriptor : IServiceDescriptor
    {
        private readonly Func<ContextualServiceProvider, object> _serviceFactory;

        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }

        public ConstructableDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            if (!serviceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException(
                    $"Implementation type '{implementationType.Name}' is not assignable to '{serviceType.Name}'",
                    nameof(implementationType));
            }

            if (DescriptorUtility.IsMonoBehavior(implementationType))
            {
                throw new ArgumentException(
                    $"Cannot register the constructable service '{serviceType.FullName}'. The implementation " +
                    $"'{implementationType.FullName}' is derived from '{typeof(MonoBehaviour).FullName}', but it should not be.",
                    nameof(serviceType));
            }
            
            ServiceType = serviceType;
            Lifetime = lifetime;
            _serviceFactory = GetFactory(implementationType);
        }
        
        public object ResolveService(ContextualServiceProvider serviceProvider)
        {
            var service = _serviceFactory.Invoke(serviceProvider);
            return service;
        }

        private static Func<ContextualServiceProvider, object> GetFactory(Type instanceType)
        {
            var parameterTypes = GetParameterTypes(instanceType);

            return serviceProvider =>
            {
                var parameters = serviceProvider.GetServices(parameterTypes);
                var instance = Activator.CreateInstance(instanceType, parameters);
                return instance;
            };
        }
        
        private static ParameterInfo[] GetParameterTypes(Type instanceType)
        {
            var constructors = instanceType.GetConstructors();

            if (constructors.Length == 0) return Array.Empty<ParameterInfo>();

            var parameters = constructors[0].GetParameters();
            
            for (var i = 1; i < constructors.Length; i++)
            {
                var applicantParameters = constructors[i].GetParameters();

                if (applicantParameters.Length > parameters.Length)
                {
                    parameters = applicantParameters;
                }
            }

            return parameters;
        }
    }
}