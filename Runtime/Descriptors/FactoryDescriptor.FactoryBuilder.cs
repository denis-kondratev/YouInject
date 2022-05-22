using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class FactoryDescriptor
    {
        private class FactoryBuilder
        {
            private readonly Type _resultType;
            private readonly Type[] _steadyArgTypes;
            private readonly int _argCount;
            
            public MethodInfo FactoryMethodInfo { get; }

            public FactoryBuilder(Type factoryType, Type resultType)
            {
                _resultType = resultType;
                var factoryDelegate = factoryType.GetMethod("Invoke")!;
                var factoryDelegateParameters = factoryDelegate.GetParameters();

                _steadyArgTypes = 
                    GetSteadyArgTypes(factoryDelegateParameters, resultType) 
                    ?? throw new Exception($"Cannot find the suitable constructor of the {resultType.Name} " +
                                           $"type for the {factoryType.Name} factory.");

                _argCount = factoryDelegateParameters.Length + _steadyArgTypes.Length;
                var delegateArgTypes = factoryDelegateParameters.Select(p => p.ParameterType).ToArray();
                FactoryMethodInfo = GetFactoryMethodInfo(factoryDelegate.ReturnType, delegateArgTypes);
            }

            public Factory BuildFactory(ServiceProvider serviceProvider)
            {
                var steadyArgs = serviceProvider.GetDecisions(_steadyArgTypes);
                var factory = new Factory(_resultType, steadyArgs, _argCount);
                
                return factory;
            }

            private static Type[]? GetSteadyArgTypes(ParameterInfo[] factoryParameters, Type resultType)
            {
                var suitableParameters = GetSuitableParameters(factoryParameters, resultType);

                if (suitableParameters is null) return null;
                
                var steadyArgTypes = GetParameterTypes(suitableParameters, factoryParameters.Length);
                return steadyArgTypes;
            }

            private static ParameterInfo[]? GetSuitableParameters(ParameterInfo[] factoryParameters, Type resultType)
            {
                var resultConstructors = resultType.GetConstructors();
                ParameterInfo[]? suitableParameters = null;
                
                foreach (var constructor in resultConstructors)
                {
                    var parameters = constructor.GetParameters();
                    
                    if (!IsParametersSuitable(factoryParameters, parameters)) continue;

                    if (suitableParameters is null || parameters.Length > suitableParameters.Length)
                    {
                        suitableParameters = parameters;
                    }
                }

                return suitableParameters;
            }

            private static bool IsParametersSuitable(ParameterInfo[] factoryParameters, ParameterInfo[] parameters)
            {
                if (factoryParameters.Length > parameters.Length) return false;

                for (var i = 0; i < factoryParameters.Length; i++)
                {
                    if (factoryParameters[i].ParameterType != parameters[i].ParameterType)
                    {
                        return false;
                    }
                }

                return true;
            }

            private static Type[] GetParameterTypes(ParameterInfo[] parameters, int gapLength)
            {
                Assert.IsTrue(parameters.Length >= gapLength);
                
                var resultLength = parameters.Length - gapLength;
                
                if (resultLength == 0) return Array.Empty<Type>();

                var types = new Type[resultLength];
                
                for (var i = 0; i < resultLength; i++)
                {
                    types[i] = parameters[gapLength + i].ParameterType;
                }

                return types;
            }

            private MethodInfo GetFactoryMethodInfo(Type delegateResultType, Type[] delegateArgTypes)
            {
                var genericTypes = new Type[delegateArgTypes.Length + 1];
                Array.Copy(delegateArgTypes, genericTypes, delegateArgTypes.Length);
                genericTypes[^1] = delegateResultType;
                var genericMethod = Factory.FactoryMethods[delegateArgTypes.Length];
                var factoryMethod = genericMethod.MakeGenericMethod(genericTypes);
                return factoryMethod;
            }
        }
    }
}