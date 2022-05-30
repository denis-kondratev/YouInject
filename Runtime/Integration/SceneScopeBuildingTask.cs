using System;
using System.Runtime.CompilerServices;

namespace YouInject
{
    public struct SceneScopeBuildingTask
    {
        private readonly SceneScopeBuilder _builder;

        internal SceneScopeBuildingTask(SceneScopeBuilder builder)
        {
            _builder = builder;
            Completed = null;
            _builder.Completed += OnCompleted;
        }

        public event Action<IScope?>? Completed;

        public Awaiter GetAwaiter()
        {
            return new Awaiter(_builder);
        }

        private void OnCompleted(SceneScope? scope)
        {
            Completed?.Invoke(scope);
        }

        public struct Awaiter : INotifyCompletion
        {
            private readonly SceneScopeBuilder _builder;
            private Action? _continuation;

            internal Awaiter(SceneScopeBuilder builder)
            {
                _builder = builder;
                _continuation = null;
            }

            public bool IsCompleted => _builder.IsUsed;

            public IScope? GetResult() => _builder.Result;
        
            public void OnCompleted(Action continuation)
            {
                _continuation = continuation;
                _builder.Completed += OnBuilderUsed;
            }

            private void OnBuilderUsed(SceneScope? scope)
            {
                _builder.Completed -= OnBuilderUsed;
                _continuation?.Invoke();
            }
        }
    }
}