using System;
using System.Collections.Generic;

namespace DEF.SyncDb;

[DEF.ComponentImpl]
public partial class ComItemEquipment : Component<IComponentStateItemEquipment>
{
    public override void Awake(Dictionary<string, object> create_params)
    {
        Console.WriteLine("ComItemEquipment Awake");
    }

    public override void OnStart()
    {
        Console.WriteLine("ComItemEquipment OnStart");
    }

    public override void OnDestroy(string reason = null, byte[] user_data = null)
    {
        UnListenAllEvent();

        Console.WriteLine("ComItemEquipment OnDestroy");
    }

    public override void HandleSelfEvent(DEF.SelfEvent ev)
    {
    }

    public override void HandleEvent(Event ev)
    {
    }
}