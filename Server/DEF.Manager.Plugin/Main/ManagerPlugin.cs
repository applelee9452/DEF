using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace DEF.Manager;

public class ManagerPlugin
{
    public Type EntryType { get; set; }
    public Assembly Ass { get; set; }
    AssemblyLoadContext AssemblyLoadContext { get; set; }
    string AssPath { get; set; }

    public ManagerPlugin(IServiceCollection services, string ass_path, string ass_name, string ass_entry_type)
    {
        string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ass_path);
        AssPath = Path.GetFullPath(p);

        AssemblyLoadContext = AssemblyLoadContext.Default;
        AssemblyLoadContext.Resolving += ContextResolving;

        using var fs = new FileStream(AssPath + ass_name + ".dll", FileMode.Open);

        Ass = AssemblyLoadContext.LoadFromStream(fs);
        EntryType = Ass.GetType(ass_entry_type);

        services.AddSingleton(EntryType);
    }

    Assembly ContextResolving(AssemblyLoadContext context, AssemblyName ass_name)
    {
        string expected_path = Path.Combine(AssPath, ass_name.Name + ".dll");
        if (File.Exists(expected_path))
        {
            try
            {
                using var fs = new FileStream(expected_path, FileMode.Open);

                return context.LoadFromStream(fs);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"加载节点{expected_path}发生异常：{ex.Message},{ex.StackTrace}");
            }
        }
        else
        {
            //Console.WriteLine($"依赖文件不存在：{expected_path}");
        }

        return null;
    }
}