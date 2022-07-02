using InjectReady.YouInject.Internal;

namespace InjectReady.YouInject
{
    public static class Host
    {
        public static IHostBuilder CreateBuilder()
        {
            return new HostBuilder();
        }
    }
}