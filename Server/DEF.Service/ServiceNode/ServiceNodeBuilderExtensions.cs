using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF;

public static class ServiceNodeBuilderExtensions
{
    public static IServiceNodeBuilder ConfigureServices(this IServiceNodeBuilder builder, Action<IServiceCollection> configureDelegate)
    {
        if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));
        configureDelegate(builder.Services);
        return builder;
    }

    public static IServiceNodeBuilder Configure<TOptions>(this IServiceNodeBuilder builder, Action<TOptions> configureOptions) where TOptions : class
    {
        return builder.ConfigureServices(services => services.Configure(configureOptions));
    }

    public static IServiceNodeBuilder Configure<TOptions>(this IServiceNodeBuilder builder, IConfiguration configuration) where TOptions : class
    {
        return builder.ConfigureServices(services => services.AddOptions<TOptions>().Bind(configuration));
    }

    public static IServiceNodeBuilder ConfigureLogging(this IServiceNodeBuilder builder, Action<ILoggingBuilder> configureLogging)
    {
        return builder.ConfigureServices(collection => collection.AddLogging(configureLogging));
    }
}