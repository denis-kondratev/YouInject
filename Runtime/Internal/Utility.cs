using System;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal static class Utility
    {
        public static bool IsComponentType(Type value)
        {
            return value.IsSubclassOf(typeof(MonoBehaviour)) && !value.IsAbstract;
        }
    }
}