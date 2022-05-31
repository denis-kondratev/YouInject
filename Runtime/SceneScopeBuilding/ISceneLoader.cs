using UnityEngine;
using UnityEngine.SceneManagement;

namespace YouInject
{
    public interface ISceneLoader
    {
        bool IsSceneLoaded(string sceneId);
        AsyncOperation LoadSceneAsync(string sceneId);
    }

    internal class DefaultSceneLoader : ISceneLoader
    {
        public bool IsSceneLoaded(string sceneId)
        {
            var scene = SceneManager.GetSceneByName(sceneId);
            return scene.isLoaded;
        }

        public AsyncOperation LoadSceneAsync(string sceneId)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
            return asyncOperation;
        }
    }
}