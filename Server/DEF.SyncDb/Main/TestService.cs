using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.SyncDb;

internal class TestService : IHostedService
{
    ILogger Logger { get; set; }
    TestContext TestContext { get; set; }

    public TestService(ILogger<TestService> logger, TestContext test_context)
    {
        Logger = logger;
        TestContext = test_context;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1);

        Logger.LogInformation("TestService启动成功！");

        await TestContext.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1);

        await TestContext.StopAsync(cancellationToken);

        Logger.LogInformation("TestService停止成功！");
    }
}