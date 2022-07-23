using System;
using System.Linq;
using System.Reflection;

namespace InjectReady.YouInject.Internal
{
    internal partial class DelegateFactoryDescriptor
    {
        private class FactoryBuilder
        {
            private readonly Type _delegateType;
            private readonly Type _productType;
            private readonly Type[] _steadyParameterTypes;
            private readonly int _totalParameterCount;
            private readonly MethodInfo _factoryMethodInfo;
            
            internal FactoryBuilder(Type delegateType, Type productType)
            {
                _delegateType = delegateType;
                _productType = productType;
                var factoryDelegate = delegateType.GetMethod("Invoke")!;

                if (productType.IsAssignableFrom(factoryDelegate.ReturnType))
                {
                    throw new DelegateFactoryRegistrationException(
                        delegateType,
                        $"The specified product type '{productType.Name}' of the factory is not assignable "
                        + $"from the returned type '{factoryDelegate.ReturnType.Name}' of specified delegate.");
                }
                
                var delegateParameters = factoryDelegate.GetParameters();
                _steadyParameterTypes = GetSteadyParameterTypes(delegateParameters, productType)
                                        ?? ThrowCannotFindSuitableConstructor(delegateType, productType);
                _totalParameterCount = delegateParameters.Length + _steadyParameterTypes.Length;
                var delegateParameterTypes = delegateParameters.Select(p => p.ParameterType).ToArray();
                _factoryMethodInfo = GetFactoryMethodInfo(factoryDelegate.ReturnType, delegateParameterTypes);
            }

            public object CreateFactoryDelegate(ServiceProvider serviceProvider, ScopeContext scopeContext)
            {
                var factory = BuildFactory(serviceProvider, scopeContext);
                var factoryDelegate = Delegate.CreateDelegate(_delegateType, factory, _factoryMethodInfo, true);
                return factoryDelegate!;
            }
            
            private GenericFactory BuildFactory(ServiceProvider serviceProvider, ScopeContext scopeContext)
            {
                var steadyParameters = new object[_steadyParameterTypes.Length];
                serviceProvider.GetServices(scopeContext, _steadyParameterTypes, steadyParameters);
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

            private static Type[] ThrowCannotFindSuitableConstructor(Type delegateType, Type productType)
            {
                throw new DelegateFactoryRegistrationException(
                    delegateType,
                    $"Cannot find the suitable constructor for the specified '{productType.Name}' "
                    + "product type of the factory.");
            }
        }
    }
}