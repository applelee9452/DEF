using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace DEF.CodeGenerator
{
    [Generator]
    public class ComponentObserverRpcCallerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new DerivedInterfacesReceiver("DEF.IComponentRpcObserver"));
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if LIB_3_11
            if (!(context.SyntaxContextReceiver is DerivedInterfacesReceiver receiver))
            {
                return;
            }
#else
            if (!(context.SyntaxReceiver is DerivedInterfacesReceiver receiver))
            {
                return;
            }
            receiver.ParseGeneratorExecutionContext(context);

#endif
            var interfaces = receiver.Interfaces;
            if (interfaces == null || interfaces.Count == 0)
            {
                return;
            }

            foreach (var i in interfaces)
            {
                var symbol = i;

                string namespacename = symbol.ContainingNamespace.ToDisplayString();
                string impl_classname = "Gen" + symbol.Name.TrimStart('I');
                string interface_name = symbol.Name;
                string factory_name = impl_classname + "Factory";
                string filename_prefix = symbol.Name.TrimStart('I');

                StringBuilder sb_methods = new StringBuilder(512);
                var members = symbol.GetMembers();
                foreach (var m in members)
                {
                    IMethodSymbol method = (IMethodSymbol)m;
                    var parameters = method.Parameters;
                    var returns = (INamedTypeSymbol)method.ReturnType;
                    var returns_typearguments = returns.TypeArguments;
                    ITypeSymbol returns_typeargument = null;
                    if (returns_typearguments.Length == 1)
                    {
                        returns_typeargument = returns_typearguments[0];
                    }

                    sb_methods.AppendLine();
                    sb_methods.AppendLine();
                    sb_methods.Append("    public ");
                    sb_methods.Append(returns.ToString());
                    sb_methods.Append(" ");
                    sb_methods.Append(method.MetadataName);
                    sb_methods.Append("(");
                    int param_index = 0;
                    foreach (var p in parameters)
                    {
                        //sb_methods.Append(p.ToString());
                        sb_methods.Append(p.Type.ToString());
                        sb_methods.Append(" ");
                        sb_methods.Append(p.MetadataName.ToString());
                        param_index++;
                        if (param_index < parameters.Length)
                        {
                            sb_methods.Append(", ");
                        }
                    }
                    sb_methods.Append(")");
                    sb_methods.AppendLine($@"
    {{");
                    sb_methods.Append($@"       return Rpcer.RequestResponse");

                    if (returns_typeargument != null || parameters.Length > 0)
                    {
                        sb_methods.Append($@"<");

                        param_index = 0;
                        foreach (var p in parameters)
                        {
                            sb_methods.Append(p.Type.ToString());
                            param_index++;
                            if (param_index < parameters.Length)
                            {
                                sb_methods.Append(", ");
                            }
                        }

                        if (parameters.Length > 0 && returns_typeargument != null)
                        {
                            sb_methods.Append($@",");
                        }

                        if (returns_typeargument != null)
                        {
                            sb_methods.Append(returns_typeargument.ToString());
                        }

                        sb_methods.Append($@">");
                    }

                    sb_methods.Append($@"(RpcInfo, ""{method.MetadataName}""");

                    foreach (var p in parameters)
                    {
                        sb_methods.Append(", ");
                        sb_methods.Append(p.MetadataName);
                    }

                    sb_methods.Append($@");");

                    sb_methods.Append($@"
    }}");
                }

                StringBuilder sb = new StringBuilder(1024);
                sb.Append($@"
using DEF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace {namespacename} {{

[RegisterSkip]
public class {impl_classname} : {interface_name}
{{
    readonly IRpcer Rpcer;
    readonly IRpcInfo RpcInfo;

    public {impl_classname}(IRpcInfo rpcinfo, IRpcer rpcer)
    {{
        RpcInfo = rpcinfo;
        Rpcer = rpcer;
    }}");
                sb.Append(sb_methods.ToString());
                sb.Append($@"
}}

public class {factory_name} : ComponentRpcObserverCallerFactory
{{
    public override string GetName()
    {{
        var t = typeof({interface_name});
        return t.Name;
    }}

    public override ComponentRpcObserverAttribute GetComponentRpcObserverAttribute()
    {{
        var t = typeof({interface_name});
        return DEFUtils.GetComponentRpcObserverAttribute(t);
    }}

    public override IComponentRpcObserver CreateComponentRpcObserver(IRpcInfo rpcinfo, IRpcer rpcer)
    {{
        return new {impl_classname}(rpcinfo, rpcer);
    }}
}}
}}");

                SourceText sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                context.AddSource(filename_prefix + ".gen.cs", sourceText);
            }
        }
    }
}