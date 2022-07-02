using System;
using UnityEngine;

namespace YouInject.Internal
{
    internal static class DescriptorUtility
    {
        private static readonly Type MonoBehaviourType = typeof(MonoBehaviour);

        public static bool IsMonoBehavior(Type type)
        {
            return MonoBehaviourType.IsAssignableFrom(type);
        }
    }
}