using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DEF.CodeGenerator
{
    public class PropInfo
    {
        public IPropertySymbol Prop { get; set; }
        public int PropType { get; set; }// 0，默认类型，1，自定义对象类型
        public int SyncFlag { get; set; }
        public int SyncMode { get; set; }
        public int Callback { get; set; }
        public string DefaultValue { get; set; }
    }

    [Generator]
    public class ComponentStateImplGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }
#endif

            //context.RegisterForPostInitialization(PostInitialization);
            //context.RegisterForSyntaxNotifications(() => new InterfaceSyntaxReceiver());
            context.RegisterForSyntaxNotifications(() => new DerivedInterfacesReceiver("DEF.IComponentState"));
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
                var symbol = (INamedTypeSymbol)i;

                var members = symbol.GetMembers();
                var propertys = new List<PropInfo>();
                foreach (var m in members)
                {
                    if (m is IPropertySymbol ps)
                    {
                        var arr_attribute = ps.GetAttributes();
                        var attr = arr_attribute[0];
                        var arg0 = attr.ConstructorArguments[0];
                        var arg1 = attr.ConstructorArguments[1];
                        var arg2 = attr.ConstructorArguments[2];
                        var arg3 = attr.ConstructorArguments[3];
                        var arg4 = attr.ConstructorArguments[4];

                        var prop_info = new PropInfo()
                        {
                            Prop = ps,
                            PropType = (int)arg0.Value,
                            SyncFlag = (int)arg1.Value,
                            SyncMode = (int)arg2.Value,
                            Callback = (int)arg3.Value,
                            DefaultValue = (string)arg4.Value,
                        };

                        propertys.Add(prop_info);
                    }
                }

                string namespacename = symbol.ContainingNamespace.ToDisplayString();
                string impl_classname = "Gen" + symbol.Name.TrimStart('I');
                string interface_name = symbol.Name;
                string factory_name = impl_classname + "Factory";
                string filename_prefix = symbol.Name.TrimStart('I');

                StringBuilder sb = new StringBuilder(1024);
                sb.Append($@"
#if !DEF_CLIENT
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
#else
using UnityEngine;
#endif
using DEF;
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace {namespacename} {{

#if !DEF_CLIENT
[BsonIgnoreExtraElements]
[BsonDiscriminator(""{impl_classname}"")]
#endif
[ProtoContract]
[MemoryPackable]
public partial class {impl_classname} : {interface_name}
{{
#if !DEF_CLIENT
    [BsonIgnore]
#endif
    public DEF.Component Component {{ get; set; }}
");

                int p_index = 0;
                foreach (var property in propertys)
                {
                    var p_name = property.Prop.Name;
                    var p_type = property.Prop.Type.ToDisplayString();

                    if (property.SyncFlag == 1)
                    {
                        // SyncDbOnly==1

                        sb.AppendLine();
                        sb.AppendLine($@"    [ProtoIgnore]");
                        sb.AppendLine("#if !DEF_CLIENT");
                        sb.AppendLine($@"    [BsonElement(""{p_name}"")]");
                        sb.AppendLine("#endif");

                        if (p_type.Contains("System.Collections.Generic.Dictionary"))
                        {
                            sb.AppendLine("#if !DEF_CLIENT");
                            sb.AppendLine($@"    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]");
                            sb.AppendLine("#endif");
                        }
                    }
                    else if (property.SyncFlag == 2)
                    {
                        // SyncNetworkOnly==2

                        sb.AppendLine();
                        sb.AppendLine("#if !DEF_CLIENT");
                        sb.AppendLine($@"    [BsonIgnore]");
                        sb.AppendLine("#endif");
                        sb.AppendLine($@"    [ProtoMember({++p_index})]");
                    }
                    else if (property.SyncFlag == 3)
                    {
                        // SyncDbAndNetwork==3

                        sb.AppendLine();
                        sb.AppendLine($@"    [ProtoMember({++p_index})]");
                        sb.AppendLine("#if !DEF_CLIENT");
                        sb.AppendLine($@"    [BsonElement(""{p_name}"")]");
                        sb.AppendLine("#endif");

                        if (p_type.Contains("System.Collections.Generic.Dictionary"))
                        {
                            sb.AppendLine("#if !DEF_CLIENT");
                            sb.AppendLine($@"    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]");
                            sb.AppendLine("#endif");
                        }
                    }
                    else
                    {
                        // SyncNone==0

                        sb.AppendLine();
                        sb.AppendLine("#if !DEF_CLIENT");
                        sb.AppendLine($@"    [BsonIgnore]");
                        sb.AppendLine("#endif");
                        sb.AppendLine($@"    [ProtoIgnore]");
                    }

                    sb.AppendLine($@"    public {p_type} {p_name} {{ get {{ return _{p_name}; }} set {{");
                    sb.AppendLine($@"       _{p_name} = value;");
                    if (property.SyncFlag == 2 || property.SyncFlag == 3)
                    {
                        sb.AppendLine("#if !DEF_CLIENT");
                        sb.AppendLine($@"       if(Component!=null && Component.Entity.NetworkSyncFlag) {{");
                        sb.AppendLine($@"           Component.Scene.WriteNetworkSyncBinlogUpdateState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, ""{p_name}"", EntitySerializer.Serialize(Component.Scene.SerializerType, _{p_name}));");
                        sb.AppendLine($"       }}");
                        sb.AppendLine("#endif");
                    }

                    if (property.PropType == 0 && property.Callback == 1)
                    {
                        sb.AppendLine($@"       On{p_name}Changed?.Invoke(_{p_name});");
                    }
                    sb.AppendLine($@"   }} }}");

                    sb.AppendLine();

                    sb.AppendLine("#if !DEF_CLIENT");
                    sb.AppendLine($@"    [BsonIgnore]");
                    sb.AppendLine("#endif");
                    sb.Append($@"    private {p_type} _{p_name}");

                    if (property.PropType == 0 && !string.IsNullOrEmpty(property.DefaultValue))
                    {
                        sb.AppendLine($@" = {property.DefaultValue};");
                    }
                    else
                    {
                        sb.AppendLine($@";");
                    }

                    if (property.PropType == 0 && property.Callback == 1)
                    {
                        sb.AppendLine("#if !DEF_CLIENT");
                        sb.AppendLine($@"    [BsonIgnore]");
                        sb.AppendLine("#endif");
                        sb.Append($@"    public OnPropChanged<{p_type}> On{p_name}Changed {{ get; set; }}");
                    }

                    sb.AppendLine();

                    if (property.SyncMode == 1)
                    {
                        sb.AppendLine("#if DEF_CLIENT");
                        sb.AppendLine($@"    [ProtoIgnore]");
                        sb.AppendLine($@"    public SyncLerp<{p_type}> SyncLerp{p_name} {{ get; set; }}");
                        sb.AppendLine("#endif");

                        sb.AppendLine();
                    }
                }

                sb.AppendLine();
                sb.AppendLine("#if DEF_CLIENT");
                sb.AppendLine($@"   public void ApplyDirtyState(string key, byte[] value) {{");
                bool first = true;
                foreach (var property in propertys)
                {
                    var p_name = property.Prop.Name;
                    var p_type = property.Prop.Type.ToDisplayString();
                    var p_sync_mode = property.SyncMode;

                    string s = "else if";
                    if (first)
                    {
                        first = false;
                        s = "if";
                    }
                    sb.AppendLine($@"       {s}(key==""{p_name}"") {{");
                    sb.AppendLine($@"           var v = EntitySerializer.Deserialize<{p_type}>(Component.Scene.SerializerType, value);");

                    if (p_sync_mode == 0)
                    {
                        // PropSyncMode.Set

                        sb.AppendLine($@"           {p_name} = v;");
                    }
                    else if (p_sync_mode == 1)
                    {
                        // PropSyncMode.Lerp

                        sb.AppendLine($@"           {p_name} = v;");
                        sb.AppendLine($@"           SyncLerp{p_name}.SetPosLastest(v);");
                    }
                    //sb.AppendLine($@"           if(sync_mode == PropSyncMode.Set) {p_name} = v;");
                    //sb.AppendLine($@"           else if(sync_mode == PropSyncMode.Lerp) {{");
                    //sb.AppendLine($@"               //var et = Component.Entity;");
                    //sb.AppendLine($@"               List{p_name}.Add(v);");
                    //sb.AppendLine($@"           }}");
                    sb.AppendLine($@"      }}");
                }
                sb.AppendLine($@"   }}");

                sb.AppendLine();

                sb.AppendLine($@"   public void ApplyDirtyCustomState(string key, byte cmd, byte[] value) {{");
                first = true;
                foreach (var property in propertys)
                {
                    var p_name = property.Prop.Name;
                    var p_type = property.Prop.Type.ToDisplayString();
                    var p_sync_mode = property.SyncMode;

                    if (property.PropType == 0) continue;

                    string s = "else if";
                    if (first)
                    {
                        first = false;
                        s = "if";
                    }
                    sb.AppendLine($@"       {s}(key==""{p_name}"") {{");

                    sb.AppendLine($@"           {p_name}.ApplyDirtyCustomState(cmd, value);");

                    sb.AppendLine($@"      }}");
                }
                sb.AppendLine($@"   }}");

                sb.AppendLine("#endif");

                sb.AppendLine();
                sb.AppendLine($@"   public void Init(DEF.Component com) {{");
                sb.AppendLine($@"       Component = com;");
                sb.AppendLine();
                foreach (var property in propertys)
                {
                    var p_name = property.Prop.Name;
                    var p_type = property.Prop.Type.ToDisplayString();

                    if (property.PropType == 1)
                    {
                        // 自定义类型

                        sb.AppendLine($@"       if(_{p_name} == null) {{");
                        sb.AppendLine($@"           _{p_name} = new();");
                        sb.AppendLine($@"       }}");
                        sb.AppendLine($@"       _{p_name}.Init(Component, ""{p_name}"");");
                        if (!string.IsNullOrEmpty(property.DefaultValue))
                        {
                            sb.AppendLine($@"           _{p_name}.SetDefaultValue({property.DefaultValue}); ");
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine("#if DEF_CLIENT");
                    if (property.SyncMode == 1)
                    {
                        // PropSyncMode.Lerp

                        sb.AppendLine($@"       SyncLerp{p_name} = new SyncLerp<{p_type}>({p_name}, {p_name});");
                        sb.AppendLine($@"       Component.Scene.AddLerp(SyncLerp{p_name});");
                        sb.AppendLine();
                    }
                    sb.AppendLine("#endif");
                }
                sb.AppendLine($@"   }}");

                sb.AppendLine();
                sb.AppendLine($@"   public void Release() {{");
                sb.AppendLine();
                sb.AppendLine("#if DEF_CLIENT");
                foreach (var property in propertys)
                {
                    var p_name = property.Prop.Name;
                    var p_type = property.Prop.Type.ToDisplayString();

                    if (property.PropType == 1)
                    {
                        // 自定义类型

                        sb.AppendLine($@"       _{p_name}.Release();");
                        sb.AppendLine();
                    }

                    if (property.SyncMode == 1)
                    {
                        // PropSyncMode.Lerp

                        sb.AppendLine($@"       Component.Scene.RemoveLerp(SyncLerp{p_name});");
                        sb.AppendLine();
                    }
                }
                sb.AppendLine("#endif");
                sb.AppendLine();
                sb.AppendLine($@"   }}");

                sb.AppendLine($@"}}");

                sb.Append($@"

public partial interface {interface_name} : IComponentState
{{
");
                foreach (var property in propertys)
                {
                    var p_name = property.Prop.Name;
                    var p_type = property.Prop.Type.ToDisplayString();
                    var p_sync_mode = property.SyncMode;

                    if (property.PropType == 0 && property.Callback == 1)
                    {
                        sb.AppendLine($@"    OnPropChanged<{p_type}> On{p_name}Changed {{ get; set; }}");
                    }

                    if (p_sync_mode == 1)
                    {
                        // PropSyncMode.Lerp

                        sb.AppendLine("#if DEF_CLIENT");
                        sb.AppendLine($@"    [ProtoIgnore]");
                        sb.AppendLine($@"    SyncLerp<{p_type}> SyncLerp{p_name} {{ get; set; }}");
                        sb.AppendLine("#endif");
                    }
                }

                sb.Append($@"}}
");

                sb.Append($@"
[RegisterComponentStateFactory]
public class {factory_name} : ComponentStateFactory
{{
    public override string GetName()
    {{
#if DEF_CLIENT
        var t = typeof({interface_name});
        var arr = t.GetCustomAttributes(false);
#else
        var t = typeof({interface_name});
        var arr = Attribute.GetCustomAttributes(t, false);
#endif

        if (arr != null)
        {{
            foreach (var i in arr)
            {{
                if (i == null) continue;

                if (i is ComponentAttribute ca)
                {{
                    return ca.ComponentName;
                }}
            }}
        }}

        return string.Empty;
    }}

    public override string GetName2()
    {{
        var t = typeof({interface_name});
        return t.Name;
    }}

    public override IComponentState CreateState(DEF.Component com, EntityStateSourceType source_type, object source)
    {{
        if (source_type == EntityStateSourceType.ProtoBuf)
        {{
            var state = EntitySerializer.Deserialize<{impl_classname}>(com.Scene.SerializerType, (byte[])source);
            state.Init(com);
            return state;
        }}
        else if (source_type == EntityStateSourceType.BsonDocument)
        {{
#if !DEF_CLIENT
            var state = ({impl_classname})BsonSerializer.Deserialize((BsonDocument)source, typeof({impl_classname}));
            state.Init(com);
            return state;
#endif
        }}

        var state1 = new {impl_classname}();
        state1.Init(com);
        return state1;
    }}
}}
}}");

                SourceText sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                context.AddSource(filename_prefix + ".gen.cs", sourceText);
            }
        }
    }
}
