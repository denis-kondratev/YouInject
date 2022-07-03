using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class DefaultLogger : IYouInjectLogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
        }
    }
}