using System;
using System.Linq;

namespace YouInject
{
    internal class NativeSpecifier : ServiceSpecifier
    {
        protected override Type[] ParameterTypes { get; }

        public NativeSpecifier(Type serviceType, Type decisionType, ServiceLifetime lifetime) 
            : base(serviceType, decisionType, lifetime)
        {
            ParameterTypes = GetParameterTypes();
        }
        
        internal override object MakeDecision(ServiceProvider serviceProvider)
        {
            var args = serviceProvider.GetDecisions(ParameterTypes);
            var decision = Activator.CreateInstance(DecisionType, args);

            return decision;
        }
        
        private Type[]  GetParameterTypes()
        {
            var constructors = DecisionType.GetConstructors();

            if (constructors.Length < 1)
            {
                throw new Exception();
            }
            
            var result = constructors[0].GetParameters();

            for (var i = 1; i < constructors.Length; i++)
            {
                var parameters = constructors[i].GetParameters();

                if (parameters.Length > result.Length)
                {
                    result = parameters;
                }
            }

            return result.Select(parameter => parameter.ParameterType).ToArray();
        }
    }
}