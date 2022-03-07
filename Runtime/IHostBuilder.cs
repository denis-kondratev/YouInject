namespace YouInject
{
    public interface IHostBuilder
    {
        IServiceRegistration AddSingleton<TService>();
        IServiceRegistration AddSingleton<TService, TDecision>();
        IServiceRegistration AddScoped<TService>();
        IServiceRegistration AddScoped<TService, TDecision>();
        IServiceRegistration AddTransient<TService>();
        IServiceRegistration AddTransient<TService, TDecision>();

        IHost BuildHost();
    }
}