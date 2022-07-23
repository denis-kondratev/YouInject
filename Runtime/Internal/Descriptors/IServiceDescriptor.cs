using System;

namespace InjectReady.YouInject.Internal
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
    }
}