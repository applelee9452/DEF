using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DEF;

[Reentrant]
public class GrainCommand : Grain, IGrainCommand
{
    ILogger Logger { get; set; }
    ICommand Command { get; set; }
    Service Service { get; set; }

    public GrainCommand(ILogger<GrainCommand> logger, Service def_service, ICommand command)
    {
        Logger = logger;
        Service = def_service;
        Command = command;
    }

    public override async Task OnActivateAsync(CancellationToken cancellation_token)
    {
        await base.OnActivateAsync(cancellation_token);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellation_token)
    {
        await base.OnDeactivateAsync(reason, cancellation_token);
    }

    Task IGrainCommand.Command(string cmd, string[] args)
    {
        if (Command != null)
        {
            return Command.Command(cmd, args);
        }

        return Task.CompletedTask;
    }
}