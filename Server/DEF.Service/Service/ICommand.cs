using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DEF;

public interface ICommand
{
    Task Command(string cmd, string[] args);
}

public class DefaultCommand : ICommand
{
    ILogger Logger { get; set; }
    Service Service { get; set; }

    public DefaultCommand(ILogger<DefaultCommand> logger, Service def_service)
    {
        Logger = logger;
        Service = def_service;
    }

    Task ICommand.Command(string cmd, string[] args)
    {
        Logger.LogInformation("请从ICommand继承并注册单例，处理cmd指令！cmd={Command}", cmd);

        return Task.CompletedTask;
    }
}