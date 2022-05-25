namespace YouInject
{
    internal partial class ServiceDescriptor
    {
        private class Builder : IServiceDescriptorBuilder
        {
            private readonly ServiceDescriptor _descriptor;

            internal Builder(ServiceDescriptor descriptor)
            {
                _descriptor = descriptor;
            }

            public void AsComponent()
            {
                _descriptor.AsComponent = true;
            }
        }
    }
}