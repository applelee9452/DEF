using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DEF;

public static class ServicesAdd
{
    static bool MemoryCacheAdded = false;
    static bool HttpClientAdded = false;

    public static void AddMemoryCache(IServiceCollection services)
    {
        if (MemoryCacheAdded) return;
        MemoryCacheAdded = true;

        services.AddMemoryCache();
    }

    public static void AddHttpClient(IServiceCollection services)
    {
        if (HttpClientAdded) return;
        HttpClientAdded = true;

        services.AddHttpClient();
    }
}