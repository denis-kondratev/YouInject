using System;
using System.Linq;

namespace YouInject.Internal
{
    internal class ServiceDescriptor : IServiceDescriptor
    {
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            if (!serviceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException(
                    $"Implementation type '{implementationType.Name}' is not assignable to '{serviceType.Name}'",
                    nameof(implementationType));
            }
            
            ServiceType = serviceType;
            Lifetime = lifetime;
            InstanceFactory = GetFactory(implementationType);
        }

        public Type ServiceType { get; }

        public ServiceLifetime Lifetime { get; }

        public Func<ScopeContext, object> InstanceFactory { get; }

        private static Func<ScopeContext, object> GetFactory(Type instanceType)
        {
            var parameterTypes = GetParameterTypes(instanceType);

            return context =>
            {
                var parameters = context.GetServices(parameterTypes);
                var instance = Activator.CreateInstance(instanceType, parameters);
                return instance;
            };
        }
        
        private static Type[] GetParameterTypes(Type instanceType)
        {
            var constructors = instanceType.GetConstructors();

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