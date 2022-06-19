﻿using System;

namespace YouInject.Internal
{
    internal interface IServiceContainer : IAsyncDisposable
    {
        object GetService(IServiceDescriptor descriptor, ServiceScope.Context context);
        void AddService(IServiceDescriptor descriptor, object service);
        void RemoveService(IServiceDescriptor descriptor);
    }
}