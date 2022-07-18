using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class Stockpile
    {
        private readonly Dictionary<Type, MonoBehaviour> _dictionary;

        internal Stockpile()
        {
            _dictionary = new Dictionary<Type, MonoBehaviour>();
        }
        
        internal void Add(Type componentType, MonoBehaviour component)
        {
            if (!_dictionary.TryAdd(componentType, component))
            {
                throw new InvalidComponentOperationException(
                    componentType,
                    "Cannot stockpile the component. A component with the same type has already been stockpiled.");
            }
        }

        internal MonoBehaviour PickUpComponent(Type componentType)
        {
            if (_dictionary.Remove(componentType, out var component))
            {
                return component;
            }

            throw new InvalidComponentOperationException(
                componentType,
                "Cannot get the component from the stockpile. It has not added yet.");
        }

        internal bool TryPickUpComponent(Type componentType, [MaybeNullWhen(false)] out MonoBehaviour component)
        {
            return _dictionary.Remove(componentType, out component);
        }
    }
}