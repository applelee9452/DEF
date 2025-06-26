#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

public class ContainerStatefulIMConsole : ContainerStateful, IContainerStatefulIMConsole
{
    public override Task OnCreate()
    {
        Logger.LogDebug($"ContainerStatefulConsole.OnCreate()");

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        Logger.LogDebug($"ContainerStatefulConsole.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerStatefulIMConsole.Touch()
    {
        return Task.CompletedTask;
    }

    async Task<string> IContainerStatefulIMConsole.ExcuteCmd2(string s)
    {
        string[] cmd_args = s.Split(' ');
        if (cmd_args == null || cmd_args.Length == 0)
        {
            return string.Empty;
        }

        string cmd = cmd_args[0];
        if (string.IsNullOrEmpty(cmd))
        {
            return string.Empty;
        }
        var cmd_args2 = cmd_args.Take(new Range(1, Index.End));

        string[] cmd_args3 = [.. (new List<string>(cmd_args2))];
        string result = await ((IContainerStatefulIMConsole)this).ExcuteCmd(cmd, cmd_args3);

        return result;
    }

    async Task<string> IContainerStatefulIMConsole.ExcuteCmd(string full_cmd, string[] args)
    {
        Logger.LogDebug("ContainerStatefulConsole.ExcuteCmd() {full_cmd}", full_cmd);

        string r;

        if (string.IsNullOrEmpty(full_cmd))
        {
            r = "命令为空！";
            return r;
        }

        var arr_str = full_cmd.Split(' ');
        if (arr_str.Length == 0)
        {
            r = "命令为空！";
            return r;
        }

        string cmd = arr_str[0];
        List<string> cmd_params = null;
        if (arr_str.Length > 1)
        {
            cmd_params = new();
            for (int i = 1; i < arr_str.Length; i++)
            {
                cmd_params.Add(arr_str[i]);
            }
        }

        if (cmd == "help")
        {
            r = "帮助：\n";
        }
        else if (cmd == "refreshcfg")
        {
            // 刷新配置文件

            //var c = GetContainerRpc<IContainerStatefulManager>();
            //await c.ReloadCfg();

            await Task.Delay(1);

            r = $"{full_cmd} 刷新配置文件成功！";
        }
        else
        {
            r = $"{full_cmd} 命令不存在！";
        }

        return r;
    }
}

#endif