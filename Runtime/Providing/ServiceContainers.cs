using System;

namespace YouInject
{
    internal partial class ServiceContainers : IDisposable
    {
        private static readonly Array LifetimeValues = Enum.GetValues(typeof(ServiceLifetime));
        private readonly IServiceContainer[] _containers;
        private readonly bool _isDerived;

        private ServiceContainers()
        {
            _containers = new IServiceContainer[LifetimeValues.Length];

            foreach (ServiceLifetime lifetime in LifetimeValues)
            {
                _containers[(int)lifetime] = lifetime switch
                {
                    ServiceLifetime.Transient => new TransientContainer(),
                    ServiceLifetime.Scoped => new Container(),
                    ServiceLifetime.Singleton => new Container(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        private ServiceContainers(ServiceContainers parentContainers)
        {
            _isDerived = true;
            _containers = new IServiceContainer[LifetimeValues.Length];

            foreach (ServiceLifetime lifetime in LifetimeValues)
            {
                _containers[(int)lifetime] = lifetime switch
                {
                    ServiceLifetime.Transient => new TransientContainer(),
                    ServiceLifetime.Scoped => parentContainers[lifetime].CreateDerivedContainer(),
                    ServiceLifetime.Singleton => parentContainers[lifetime],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public void Dispose()
        {
            foreach (ServiceLifetime lifetime in LifetimeValues)
            {
                if (_isDerived && lifetime == ServiceLifetime.Singleton) continue;
                
                this[lifetime].Dispose();
            }
        }

        internal static ServiceContainers CreateRootContainers()
        {
            var containers = new ServiceContainers();
            return containers;
        }

        internal ServiceContainers CreateDerivedContainers()
        {
            var derivedContainer = new ServiceContainers(this);
            return derivedContainer;
        }

        internal IServiceContainer this[ServiceLifetime lifetime] => _containers[(int)lifetime];
    }
}