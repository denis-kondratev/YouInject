using System;

namespace InjectReady.YouInject.Internal
{
    internal static class ExceptionBuilder
    {
        internal static InvalidOperationException ComponentNotRegistered(Type componentType)
        {
            return new InvalidOperationException($"The '{componentType.Name}' component is not registered");
        }
        
        internal static InvalidOperationException ServiceNotRegistered(Type serviceType)
        {
            return new InvalidOperationException($"The '{serviceType.Name}' service is not registered");
        }

        internal static InvalidOperationException ServiceNotFound(Type serviceType)
        {
            return new InvalidOperationException($"Cannot find the '{serviceType.Name}' service.");
        }

        internal static ArgumentException UnexpectedDescriptor(Type descriptorType)
        {
            return new ArgumentException($"Unexpected descriptor of the '{descriptorType.Name}' type.");
        }

        internal static ArgumentException NoSuitableConstructor(Type delegateType, Type productType)
        {
            throw new ArgumentException(
                $"Cannot find the suitable constructor of the '{productType.Name}' "
                + $"product type for the '{delegateType.Name}' delegate factory.");
        }

        internal static InvalidOperationException ObjectBaked(string objectName)
        {
            throw new InvalidOperationException($"{objectName} is already baked.");
        }

        internal static InvalidOperationException ServiceAlreadyRegistered(Type serviceType)
        {
            return new InvalidOperationException($"The '{serviceType.Name}' service is already registered.");
        }

        internal static ArgumentException InstanceDoesNotMatchBinding(Type instanceType, Type bindingType)
        {
            return new ArgumentException($"The instance type '{instanceType.Name}' does not match the binding one '{bindingType.Name}'.");
        }

        internal static InvalidOperationException ScopeDoesNotExist()
        {
            return new InvalidOperationException("The scope does not exist.");
        }
        
        internal static InvalidOperationException ScopeAlreadyExists()
        {
            return new InvalidOperationException("The scope already exists.");
        }
    }
}