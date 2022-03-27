namespace YouInject
{
    internal partial class ComponentDescriptor
    {
        private class Builder : IComponentDescriptorBuilder
        {
            private readonly ComponentDescriptor _descriptor;

            internal Builder(ComponentDescriptor descriptor)
            {
                _descriptor = descriptor;
            }
            
            public void AsTransient()
            {
                _descriptor.Lifetime = ServiceLifetime.Transient;
            }
        }
    }
}