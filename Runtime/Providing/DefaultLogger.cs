using UnityEngine;

namespace YouInject
{
    internal class DefaultLogger : IYouInjectLogger
    {
        public void Log(object message)
        {
            Debug.Log(message);
        }
    }
}