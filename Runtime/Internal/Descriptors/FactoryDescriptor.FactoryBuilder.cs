using System;
using System.Linq;
using System.Reflection;

namespace InjectReady.YouInject.Internal
{
    internal partial class DelegateFactoryDescriptor
    {
        private class FactoryBuilder
        {
            private readonly DelegateFactoryDescriptor _descriptor;
            private readonly Type[] _steadyParameterTypes;
            private readonly int _totalParameterCount;
            private readonly MethodInfo _factoryMethodInfo;
            
            public FactoryBuilder(DelegateFactoryDescriptor descriptor)
            {
                _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
                var factoryDelegate = descriptor.ServiceType.GetMethod("Invoke")!;
                var delegateParameters = factoryDelegate.GetParameters();

                _steadyParameterTypes = GetSteadyParameterTypes(delegateParameters, descriptor._productInstanceType) 
                                        ?? ThrowCannotFindSuitableConstructor(descriptor.ServiceType, descriptor._productInstanceType);
                _totalParameterCount = delegateParameters.Length + _steadyParameterTypes.Length;
                var delegateParameterTypes = delegateParameters.Select(p => p.ParameterType).ToArray();
                _factoryMethodInfo = GetFactoryMethodInfo(factoryDelegate.ReturnType, delegateParameterTypes);
            }

            public object GetFactoryDelegate(ContextualServiceProvider context)
            {
                var factory = BuildFactory(context);
                var factoryDelegate = Delegate.CreateDelegate(_descriptor.ServiceType, factory, _factoryMethodInfo, true);
                return factoryDelegate!;
            }
            
            private GenericFactory BuildFactory(ContextualServiceProvider context)
            {
                var steadyParameters = context.GetServices(_steadyParameterTypes);
                var factory = new GenericFactory(_descriptor._productInstanceType, steadyParameters, _totalParameterCount);
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