using Microsoft.Extensions.DependencyInjection;

namespace DEF;

public interface IServiceClientBuilder
{
    IServiceCollection Services { get; }
}