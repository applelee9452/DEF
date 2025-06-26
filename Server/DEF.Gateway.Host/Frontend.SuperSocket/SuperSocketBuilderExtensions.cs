using SuperSocket;
using SuperSocket.Server.Abstractions.Host;

namespace DEF.Gateway;

public static class SuperSocketBuilderExtensions
{
    public static IHostBuilder UseSuperSocket(this IHostBuilder builder, Func<IHostBuilder, ISuperSocketHostBuilder> configure_delegate)
    {
        var builder2 = configure_delegate?.Invoke(builder);

        if (builder2 == null)
        {
            return builder;
        }

        return builder2;
    }
}