using System.Diagnostics;

namespace YouInject.Internal
{
    internal class Logger
    {
        private readonly IYouInjectLogger _implementation;

        public Logger(IYouInjectLogger implementation)
        {
            _implementation = implementation;
        }

        [Conditional("YOU_INJECT_LOGGING")]
        public void Log(string message) => _implementation.Log(message);
    }
}