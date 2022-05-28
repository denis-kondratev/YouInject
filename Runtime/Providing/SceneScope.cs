using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace YouInject
{
    internal class SceneScope : Scope
    {
        private readonly string _name;
        private bool _isDisposed;

        internal SceneScope(BakedServiceCollection services, ComponentProvider componentProvider, string name, Scope parentScope) 
            : base(services, componentProvider, name, parentScope)
        {
            _name = name;
            ComponentProvider = componentProvider;
        }

        public ComponentProvider ComponentProvider { get; }

        public override async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            
            _isDisposed = true;
            var operation = SceneManager.UnloadSceneAsync(_name);

            while (!operation?.isDone ?? false)
            {
                await Task.Yield();
            }
            
            await base.DisposeAsync();
        }
    }
}