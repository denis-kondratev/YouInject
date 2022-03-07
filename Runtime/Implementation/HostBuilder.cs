using System;
using System.Collections.Generic;

namespace YouInject
{
    internal class HostBuilder : IHostBuilder
    {
        private readonly List<ServiceRegistration> _collection;
        
        internal HostBuilder()
        {
            _collection = new List<ServiceRegistration>();
        }
        
        public IServiceRegistration AddSingleton<TService>()
        {
            var serviceType = typeof(TService);

            return AddService(serviceType, serviceType, ServiceLifetime.Singleton);
        }

        public IServiceRegistration AddSingleton<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decisionType = typeof(TDecision);

            return AddService(serviceType, decisionType, ServiceLifetime.Singleton);
        }

        public IServiceRegistration AddScoped<TService>()
        {
            var serviceType = typeof(TService);

            return AddService(serviceType, serviceType, ServiceLifetime.Scoped);
        }

        public IServiceRegistration AddScoped<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decisionType = typeof(TDecision);

            return AddService(serviceType, decisionType, ServiceLifetime.Scoped);
        }

        public IServiceRegistration AddTransient<TService>()
        {
            var serviceType = typeof(TService);

            return AddService(serviceType, serviceType, ServiceLifetime.Transient);
        }

        public IServiceRegistration AddTransient<TService, TDecision>()
        {
            var serviceType = typeof(TService);
            var decisionType = typeof(TDecision);

            return AddService(serviceType, decisionType, ServiceLifetime.Transient);
        }

        public IHost BuildHost()
        {
            var serviceDescriptor = new ServiceCollection(_collection);
            var host = new Host(serviceDescriptor);

            return host;
        }

        private IServiceRegistration AddService(Type serviceType, Type decisionType, ServiceLifetime lifetime)
        {
            var registration = new ServiceRegistration(serviceType, decisionType, lifetime);
            
            _collection.Add(registration);

            return registration;
        }
    }
}