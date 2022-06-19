using System;
using System.Linq;
using System.Reflection;

namespace YouInject.Internal
{
    internal partial class FactoryDescriptor
    {
        private class FactoryBuilder
        {
            private readonly Type _factoryType;
            private readonly Type _productType;
            private readonly Type[] _steadyParameterTypes;
            private readonly int _totalParameterCount;
            private readonly MethodInfo _factoryMethodInfo;
            
            public FactoryBuilder(Type factoryType, Type productType)
            {
                _factoryType = factoryType ?? throw new ArgumentNullException(nameof(factoryType));
                _productType = productType ?? throw new ArgumentNullException(nameof(productType));
                
                var factoryDelegate = factoryType.GetMethod("Invoke")!;
                var delegateParameters = factoryDelegate.GetParameters();

                _steadyParameterTypes = GetSteadyParameterTypes(delegateParameters, productType) 
                                        ?? ThrowCannotFindSuitableConstructor(factoryType, productType);
                _totalParameterCount = delegateParameters.Length + _steadyParameterTypes.Length;
                var delegateParameterTypes = delegateParameters.Select(p => p.ParameterType).ToArray();
                _factoryMethodInfo = GetFactoryMethodInfo(factoryDelegate.ReturnType, delegateParameterTypes);
            }

            public object GetFactoryDelegate(ServiceScope.Context context)
            {
                var factory = BuildFactory(context);
                var factoryDelegate = Delegate.CreateDelegate(_factoryType, factory, _factoryMethodInfo, true);
                return factoryDelegate!;
            }
            
            private GenericFactory BuildFactory(ServiceScope.Context context)
            {
                var steadyParameters = context.GetInitializingParameters(_factoryType, _steadyParameterTypes);
                var factory = new GenericFactory(_productType, steadyParameters, _totalParameterCount);
                return factory;
            }

            private static Type[]? GetSteadyParameterTypes(ParameterInfo[] delegateParameters, Type returnType)
            {
                var totalParameters = GetTotalParameters(delegateParameters, returnType);

                if (totalParameters is null) return null;

                var steadyParameterTypes = GetParameterTypes(totalParameters, delegateParameters.Length);
                return steadyParameterTypes;
            }

            private static ParameterInfo[]? GetTotalParameters(ParameterInfo[] factoryParameters, Type resultType)
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
                var resultLength = parameters.Length - gapLength;
                
                if (resultLength == 0) return Array.Empty<Type>();

                var types = new Type[resultLength];
                
                for (var i = 0; i < resultLength; i++)
                {
                    types[i] = parameters[gapLength + i].ParameterType;
                }

                return types;
            }

            private static MethodInfo GetFactoryMethodInfo(Type resultType, Type[] parameterTypes)
            {
                var genericTypes = new Type[parameterTypes.Length + 1];
                Array.Copy(parameterTypes, genericTypes, parameterTypes.Length);
                genericTypes[^1] = resultType;
                var genericMethod = GenericFactory.FactoryMethods[parameterTypes.Length];
                var methodInfo = genericMethod.MakeGenericMethod(genericTypes);
                return methodInfo;
            }

            private static Type[] ThrowCannotFindSuitableConstructor(Type factoryType, Type productType)
            {
                throw new ArgumentException($"Cannot find the suitable constructor of the '{productType.Name}'" +
                                            $" type for the '{factoryType.Name}' factory.");
            }
        }
    }
}