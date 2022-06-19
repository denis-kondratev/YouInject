using System;
using System.Linq;

namespace YouInject.Internal
{
    internal class ServiceDescriptor : IServiceDescriptor
    {
        public ServiceDescriptor(Type serviceType, Type instanceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            Lifetime = lifetime;
            InstanceFactory = GetFactory(instanceType);
        }

        public Type ServiceType { get; }

        public ServiceLifetime Lifetime { get; }

        public Func<ServiceScope.Context, object> InstanceFactory { get; }

        private Func<ServiceScope.Context, object> GetFactory(Type instanceType)
        {
            var parameterTypes = GetParameterTypes(instanceType);

            return context =>
            {
                var parameters = context.GetInitializingParameters(ServiceType, parameterTypes);
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