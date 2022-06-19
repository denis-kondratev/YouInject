using YouInject.Internal;

namespace YouInject
{
    public static class Host
    {
        public static IHostBuilder CreateBuilder()
        {
            return new HostBuilder();
        }
    }
}