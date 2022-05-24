using System;
using System.Collections.Generic;
using UnityEngine;

namespace YouInject
{
    internal class SceneScope : Scope
    {
        private readonly ServiceProvider _serviceProvider;

        internal SceneScope(BakedServiceCollection services, ServiceProvider serviceProvider, string name, Scope parentScope) 
            : base(services, serviceProvider, name, parentScope)
        {
            _serviceProvider = serviceProvider;
        }
        internal void AddComponents(Dictionary<Type, Component> components)
        {
            _serviceProvider.AddComponents(components);
        }
        
    }
}