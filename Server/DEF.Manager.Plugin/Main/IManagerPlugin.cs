using System.Reflection;

namespace DEF.Manager;

public class PluginInfo
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string MinRole { get; set; }
    public Assembly AssemblyPlugin { get; set; }
    public Assembly AssemblyInterface { get; set; }
    public List<NavMenuItem> NavMenuItems { get; set; }
}

public class NavMenuItem
{
    public string Text { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
    public string MinRole { get; set; }
    public List<NavMenuItem> Children { get; set; } = [];
}

public interface IManagerPlugin
{
    PluginInfo GetPluginInfo();
}