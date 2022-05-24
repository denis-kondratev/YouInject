namespace YouInject
{
    internal class SceneScope : Scope
    {
        internal SceneScope(BakedServiceCollection services, ComponentProvider componentProvider, string name, Scope parentScope) 
            : base(services, componentProvider, name, parentScope)
        {
            ComponentProvider = componentProvider;
        }

        public ComponentProvider ComponentProvider { get; }
    }
}