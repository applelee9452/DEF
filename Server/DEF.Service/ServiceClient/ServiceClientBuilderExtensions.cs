using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DEF;

public static class ServiceClientBuilderExtensions
{
    public static IServiceClientBuilder ConfigureServices(this IServiceClientBuilder builder, Action<IServiceCollection> configureDelegate)
    {
        if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));
        configureDelegate(builder.Services);
        return builder;
    }

    public static IServiceClientBuilder Configure<TOptions>(this IServiceClientBuilder builder, Action<TOptions> configureOptions) where TOptions : class
    {
        return builder.ConfigureServices(services => services.Configure(configureOptions));
    }

    public static IServiceClientBuilder Configure<TOptions>(this IServiceClientBuilder builder, IConfiguration configuration) where TOptions : class
    {
        return builder.ConfigureServices(services => services.AddOptions<TOptions>().Bind(configuration));
    }

    public static IServiceClientBuilder ConfigureLogging(this IServiceClientBuilder builder, Action<ILoggingBuilder> configureLogging)
    {
        return builder.ConfigureServices(collection => collection.AddLogging(configureLogging));
    }
}