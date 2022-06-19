using UnityEngine;

namespace YouInject.Internal
{
    internal class DefaultLogger : IYouInjectLogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
        }
    }
}