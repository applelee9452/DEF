using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DEF.CodeGenerator
{
    [Generator]
    public class ComponentObserverRpcInvokeHelperGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new ClassesWithInterfacesReceiver("DEF.IComponentRpcObserver"));
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if LIB_3_11
            if (!(context.SyntaxContextReceiver is ClassesWithInterfacesReceiver receiver))
            {
                return;
            }
#else
            if (!(context.SyntaxReceiver is ClassesWithInterfacesReceiver receiver))
            {
                return;
            }
            receiver.ParseGeneratorExecutionContext(context);
#endif
            var classes = receiver.Classes;
            if (classes == null || classes.Count == 0)
            {
                return;
            }

            Dictionary<string, INamedTypeSymbol> map_interface = new Dictionary<string, INamedTypeSymbol>();
            foreach (var i in classes)
            {
                var intefaces = i.AllInterfaces;
                foreach (var iface in intefaces)
                {
                    var attrs = iface.GetAttributes();
                    foreach (var attr in attrs)
                    {
                        if (attr.AttributeClass.Name == "ComponentRpcObserverAttribute")
                        {
                            var s = attr.ToString();
                            int i1 = s.IndexOf('\"');
                            s = s.Substring(i1 + 1, s.Length - i1 - 1);
                            i1 = s.IndexOf('\"');
                            s = s.Substring(0, i1);
                            map_interface[s] = iface;
                        }
                    }
                }
            }

            foreach (var i in map_interface)
            {
                var symbol = i.Value;

                string component_name = i.Key;
                string namespacename = symbol.ContainingNamespace.ToDisplayString();
                string impl_classname = "Gen" + symbol.Name.TrimStart('I') + "InvokeHelper";
                string interface_name = symbol.Name;
                string filename_prefix = symbol.Name.TrimStart('I') + "InvokeHelper";

                StringBuilder sb_methods = new StringBuilder(512);
                var members = symbol.GetMembers();
                foreach (var m in members)
                {
                    IMethodSymbol method = (IMethodSymbol)m;

                    var parameters = method.Parameters;
                    string param_types = string.Empty;
                    string param_objs = string.Empty;
                    if (parameters.Length > 0)
                    {
                        param_types += "<";
                        int param_index = 0;
                        foreach (var pp in parameters)
                        {
                            param_types += pp.Type.ToString();
                            param_index++;

                            param_objs += "so.obj";
                            param_objs += param_index.ToString();

                            if (param_index < parameters.Length)
                            {
                                param_types += ", ";
                                param_objs += ", ";
                            }
                        }
                        param_types += ">";
                    }

                    sb_methods.AppendLine();
                    sb_methods.AppendLine();
                    sb_methods.Append("    public static Task ");
                    sb_methods.Append(method.MetadataName);
                    sb_methods.Append($@"(int int_st, IComponentRpcObserver instance, byte[] method_data)");

                    if (parameters.Length == 0)
                    {
                        sb_methods.AppendLine($@"
    {{        
        return instance.{method.MetadataName}();
    }}");
                    }
                    else
                    {
                        sb_methods.AppendLine($@"
    {{
        var st = (SerializerType)int_st;
        var so = EntitySerializer.Deserialize<SerializeObj{param_types}>(st, method_data);

        return (({interface_name})instance).{method.MetadataName}({param_objs});
    }}");
                    }
                }

                StringBuilder sb = new StringBuilder(1024);
                sb.Append($@"
using DEF;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace {namespacename} {{

[RegisterComponentRpcObserverInvokeHelper(""{component_name}"")]
public class {impl_classname}
{{");
                sb.Append(sb_methods.ToString());
                sb.Append($@"
}}
}}");

                SourceText sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                context.AddSource(filename_prefix + ".gen.cs", sourceText);
            }
        }
    }
}