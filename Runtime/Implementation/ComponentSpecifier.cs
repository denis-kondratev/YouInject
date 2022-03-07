using System;
using System.Reflection;

namespace YouInject
{
    internal class ComponentSpecifier : ServiceSpecifier
    {
        private readonly MethodInfo? _initialMethod;

        protected override Type[] ParameterTypes { get; }

        public ComponentSpecifier(Type serviceType, Type decisionType, ServiceLifetime lifetime, string? initMethodName) 
            : base(serviceType, decisionType, lifetime)
        {
            _initialMethod = string.IsNullOrEmpty(initMethodName) ? null : decisionType.GetMethod(initMethodName);
            ParameterTypes = GetParameterTypes();
        }

        private Type[] GetParameterTypes()
        {
            if (_initialMethod is null) return Array.Empty<Type>();

            var parameters = _initialMethod.GetParameters();
            var parameterTypes = new Type[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }

            return parameterTypes;
        }

        internal override object MakeDecision(ServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        internal void InitializeComponent(ServiceProvider serviceProvider, object component)
        {
            if (_initialMethod is null) return;
            
            var parameters = serviceProvider.GetDecisions(ParameterTypes);
            _initialMethod.Invoke(component, parameters);
        }
    }
}