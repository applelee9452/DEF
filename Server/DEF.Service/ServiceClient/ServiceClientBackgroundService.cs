using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DEF.Gateway;

public class ServiceClientBackgroundService : BackgroundService
{
    IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<ServiceClientOptions> ServiceClientOptions { get; set; }
    ServiceClient ServiceClient { get; set; }
    ILogger Logger { get; set; }

    public ServiceClientBackgroundService(ILogger<ServiceClient> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceClientOptions> serviceclient_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClientOptions = serviceclient_options;
        ServiceClient = service_client;
    }

    protected override async Task ExecuteAsync(CancellationToken stopping_token)
    {
        while (!stopping_token.IsCancellationRequested)
        {
            // 在这里编写需要定期执行的逻辑代码

            await Task.Delay(TimeSpan.FromSeconds(10), stopping_token);

            try
            {
                await ServiceClient.SessionTouch();

                //Logger.LogInformation("ServiceClient.SessionTouch() ~~~~~~~~~~~~~~~~~~~~~~~~~");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ServiceClient.SessionTouch() Error");
            }
        }

        Logger.LogInformation("后台服务已停止");
    }
}