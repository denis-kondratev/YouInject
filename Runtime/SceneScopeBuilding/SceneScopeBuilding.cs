using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace YouInject
{
    public class SceneScopeBuilding
    {
        private SceneScopeBuilder? _builder;

        internal SceneScopeBuilding(SceneScopeBuilder builder)
        {
            if (builder.IsCompleted)
            {
                IsCompleted = true;
                Result = builder.Result;

                return;
            }
            
            _builder = builder;
            _builder.Completed += OnCompleted;
        }

        internal SceneScopeBuilding(SceneScope scope)
        {
            IsCompleted = true;
            Result = scope;
        }

        public IScope? Result { get; private set; }

        public bool IsCompleted { get; private set; }

        public event Action? Completed;

        public Awaiter GetAwaiter()
        {
            return new Awaiter(this);
        }
        
        public async Task<IScope?>AsTask()
        {
            return await this;
        }

        private void OnCompleted(SceneScope? scope)
        {
            if (_builder is null) throw new NullReferenceException();
            
            _builder.Completed -= OnCompleted;
            Result = _builder.Result;
            IsCompleted = true;
            _builder = null!;
            Completed?.Invoke();
        }

        public struct Awaiter : INotifyCompletion
        {
            private readonly SceneScopeBuilding _building;
            private Action? _continuation;

            internal Awaiter(SceneScopeBuilding building)
            {
                _building = building;
                _continuation = null;
            }

            public bool IsCompleted => _building.IsCompleted;

            public IScope? GetResult() => _building.Result;
        
            public void OnCompleted(Action continuation)
            {
                _continuation = continuation;
                _building.Completed += OnBuilderUsed;
            }

            private void OnBuilderUsed()
            {
                _building.Completed -= OnBuilderUsed;
                _continuation?.Invoke();
            }
        }
    }
}