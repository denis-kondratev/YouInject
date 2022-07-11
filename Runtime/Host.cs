using System;
using System.Threading.Tasks;

namespace InjectReady.YouInject
{
    public static partial class Host
    {
        private static Internal.Host? _instance;

        public static IExtendedServiceProvider ServiceProvider
        {
            get
            {
                if (_instance is null)
                {
                    throw new InvalidOperationException("The host instance does not exist.");
                }
                
                return _instance.ServiceProvider;
            }
        }
        
        public static IHostBuilder CreateBuilder()
        {
            return new HostBuilder();
        }

        public static async ValueTask DisposeAsync()
        {
            if (_instance is null) return;

            await _instance.DisposeAsync();
        }
    }
}