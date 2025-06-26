using System;
using System.Collections.Generic;

namespace DEF.SyncDb;

[DEF.ComponentImpl]
public partial class ComPlayerInventory : Component<IComponentStatePlayerInventory>
{
    public override void Awake(Dictionary<string, object> create_params)
    {
        Console.WriteLine("ComPlayerInventory Awake");
    }

    public override void OnStart()
    {
        Console.WriteLine("ComPlayerInventory OnStart");
    }

    public override void OnDestroy(string reason = null, byte[] user_data = null)
    {
        UnListenAllEvent();

        Console.WriteLine("ComPlayerInventory OnDestroy");
    }

    public override void HandleSelfEvent(DEF.SelfEvent ev)
    {
    }

    public override void HandleEvent(Event ev)
    {
    }
}