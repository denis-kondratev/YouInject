using System;
using UnityEngine;

namespace YouInject
{
    internal interface ISceneScopeBuilder : IDisposable
    {
        void AddComponents(Component[] components);
    }
}