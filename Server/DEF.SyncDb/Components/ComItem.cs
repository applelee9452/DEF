using System;
using System.Collections.Generic;

namespace DEF.SyncDb;

[DEF.ComponentImpl]
public partial class ComItem : Component<IComponentStateItem>
{
    public override void Awake(Dictionary<string, object> create_params)
    {
        Console.WriteLine("ComItem Awake");
    }

    public override void OnStart()
    {
        Console.WriteLine("ComItem OnStart");
    }

    public override void OnDestroy(string reason = null, byte[] user_data = null)
    {
        UnListenAllEvent();

        Console.WriteLine("ComItem OnDestroy");
    }

    public override void HandleSelfEvent(DEF.SelfEvent ev)
    {
    }

    public override void HandleEvent(Event ev)
    {
    }
}