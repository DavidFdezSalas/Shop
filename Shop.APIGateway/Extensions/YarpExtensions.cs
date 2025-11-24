namespace Shop.APIGateway.Extensions
{
    public static class YarpExtensions
    {
        public static IServiceCollection AddYarpReverseProxy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServiceDiscovery();

            services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                .AddServiceDiscoveryDestinationResolver();

            return services;
        }

        public static IServiceCollection AddGatewayCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    {
                        policy.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    }
                });
            });
            return services;
        }
    }

}
