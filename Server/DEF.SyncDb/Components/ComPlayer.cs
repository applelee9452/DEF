using System;
using System.Collections.Generic;

namespace DEF.SyncDb;

[DEF.ComponentImpl]
public partial class ComPlayer : Component<IComponentStatePlayer>
{
    public override void Awake(Dictionary<string, object> create_params)
    {
        Console.WriteLine("ComPlayer Awake");
    }

    public override void OnStart()
    {
        Console.WriteLine("ComPlayer OnStart");
    }

    public override void OnDestroy(string reason = null, byte[] user_data = null)
    {
        UnListenAllEvent();

        Console.WriteLine("ComPlayer OnDestroy");
    }

    public override void HandleSelfEvent(DEF.SelfEvent ev)
    {
    }

    public override void HandleEvent(Event ev)
    {
    }
}