using System;
using UnityEngine;

namespace YouInject
{
    internal class ServiceRegistration : IServiceRegistration
    {
        private static readonly Type ComponentType = typeof(Component);

        private readonly Type _serviceType;
        private readonly Type _decisionType;
        private readonly ServiceLifetime _lifetime;
        private string? _initializingMethodName;
        
        internal ServiceRegistration(Type serviceType, Type decisionType, ServiceLifetime lifetime)
        {
            _serviceType = serviceType;
            _decisionType = decisionType;
            _lifetime = lifetime;
        }
        
        public void InitializeWidth(string methodName)
        {
            _initializingMethodName = methodName;
        }

        internal ServiceSpecifier BuildSpecifier()
        {
            var specifier = ComponentType.IsAssignableFrom(_decisionType)
                ? BuildComponentSpecifier()
                : BuildNativeSpecifier();

            return specifier;
        }

        private ServiceSpecifier BuildNativeSpecifier()
        {
            if (!string.IsNullOrEmpty(_initializingMethodName))
            {
                throw new Exception(
                    $"The {_serviceType.Name} service cannot have initializing method " +
                    $"{_initializingMethodName}. Only component service can have initializing method.");
            }
            
            var specifier = new NativeSpecifier(_serviceType, _decisionType, _lifetime);

            return specifier;
        }

        private ServiceSpecifier BuildComponentSpecifier()
        {
            var specifier = new ComponentSpecifier(_serviceType, _decisionType, _lifetime, _initializingMethodName);

            return specifier;
        }
    }
}