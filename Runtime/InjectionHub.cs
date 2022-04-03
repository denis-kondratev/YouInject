using System.Diagnostics.CodeAnalysis;

namespace YouInject
{
    public static partial class InjectionHub
    {
        [AllowNull] public static IHost BuiltHost { get; private set; }

        public static IHostBuilder CreateBuilder()
        {
            var builder = new HostBuilder();
            return builder;
        }
    }
}