using System;

namespace YouInject
{
    public static partial class HostUtility
    {
        private static Host? _host;
        
        public static IHostBuilder CreateHostBuilder()
        {
            var builder = new HostBuilder();
            return builder;
        }
        
        public static ISceneScopeProvider GetSceneScopeProvider()
        {
            if (_host is null)
            {
                throw new Exception("The host doesn't exist.");
            }
            
            return _host;
        }
    }
}