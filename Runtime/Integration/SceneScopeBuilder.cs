using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YouInject
{
    internal class SceneScopeBuilder
    {
        private readonly BakedServiceCollection _services;
        private readonly Dictionary<Type, Component> _componentsToAdd;
        private bool _hasBuilt;

        internal SceneScopeBuilder(BakedServiceCollection services)
        {
            _services = services;
            _componentsToAdd = new Dictionary<Type, Component>();
        }

        internal void InjectComponent(Component component)
        {
            Assert.IsFalse(_hasBuilt, $"Failed to inject a component of the '{component.GetType().Name}' " +
                                      "type, the builder has already been used.");

            if (!_services.TryGetServiceTypeByDecision(component, out var serviceType))
            {
                throw new Exception($"Failed a component injection because a service with a decision of the " +
                                    $"{component.GetType().Name} type is not registered with the host.");
            }

            _componentsToAdd.Add(serviceType, component);
        }

        internal Scope BuildScope(Scope parentScope)
        {
            Assert.IsFalse(_hasBuilt, "Failed to build a scope, the builder has already been used.");

            _hasBuilt = true;
            var scope = parentScope.CreateDerivedScopeInternally();
            scope.AddComponents(_componentsToAdd);
            return scope;
        }
    }
}