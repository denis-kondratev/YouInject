using UnityEngine;

namespace YouInject
{
    internal interface ISceneScopeBuilder
    {
        void AddComponents(Component[] components);
    }
}