using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ComponentDescriptor
    {
        private class Builder : IComponentDescriptorBuilder
        {
            private readonly ComponentDescriptor _descriptor;

            internal Builder(ComponentDescriptor descriptor)
            {
                _descriptor = descriptor;
            }

            public void InitializeWith(string methodName)
            {
                Assert.IsFalse(_descriptor._isBaked);

                var methodInfo = _descriptor.DecisionType.GetMethod(methodName);

                if (methodInfo is null)
                {
                    throw new Exception($"Cannot find '{methodName}' method in '{_descriptor.DecisionType}' type.");
                }

                _descriptor._initializingParameterTypes = GetMethodParameterTypes(methodInfo);
                _descriptor._initialisingMethod = methodInfo;
            }
            
            private static Type[] GetMethodParameterTypes(MethodBase method)
            {
                var parameters = method.GetParameters();
                var types = parameters.Select(parameter => parameter.ParameterType).ToArray();
                return types;
            }
        }
    }
}