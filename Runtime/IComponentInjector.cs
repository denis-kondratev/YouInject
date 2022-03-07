using System;
using System.Collections.Generic;

namespace YouInject
{
    public interface IComponentInjector
    {
        void InjectComponent(Type serviceType, object decision);

        void InitializeInjectedComponents();
        
        IEnumerable<(Type serviceType, Type decisionType)> GetUnimplementedServices();
    }
}