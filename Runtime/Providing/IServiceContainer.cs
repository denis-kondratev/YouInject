using System;
using System.Diagnostics.CodeAnalysis;

namespace YouInject
{
    internal interface IServiceContainer : IDisposable
    {
        void AddDecision(object decision, Type serviceType);
        bool TryGetDecision(Type serviceType, [NotNullWhen(true)] out object? decision);
        IServiceContainer CreateDerivedContainer();
    }
}