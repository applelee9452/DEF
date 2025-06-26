using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DEF.Manager;

public class ManagerPlugins
{
    public List<Assembly> Assemblys { get; set; } = [];
    public List<Type> EntryTypes { get; set; } = [];
    public List<MenuItem> NavMenuItems { get; set; } = [];
    ManagerOptions ManagerOptions { get; set; }

    public ManagerPlugins(IServiceCollection services, ManagerOptions manager_options)
    {
        ManagerOptions = manager_options;

        foreach (var i in ManagerOptions.Plugins)
        {
            ManagerPlugin plugin = new(services, i.AssemblyPath, i.AssemblyName, i.EntryType);

            Assemblys.Add(plugin.Ass);

            EntryTypes.Add(plugin.EntryType);
        }
    }
}