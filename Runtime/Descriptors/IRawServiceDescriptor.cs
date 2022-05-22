using System;

namespace YouInject
{
    internal interface IRawServiceDescriptor
    {
        IServiceDescriptor Bake();
        Type ServiceType { get; }
    }
}