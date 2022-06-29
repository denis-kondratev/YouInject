using System;
using UnityEngine;

namespace YouInject.Internal
{
    internal static class DescriptorUtility
    {
        private static readonly Type ComponentType = typeof(Component);

        public static bool IsComponent(Type type)
        {
            return ComponentType.IsAssignableFrom(type);
        }
    }
}