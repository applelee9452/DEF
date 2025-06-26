using System.Collections.Generic;

namespace DEF
{
    public class TlSelfEvent : DEF.SelfEvent
    {
        public Dictionary<string, object> input { get; set; } = new();
        public Dictionary<string, object> output { get; set; } = new();
    }

    public class TlEvent : DEF.Event
    {
        public Dictionary<string, object> input { get; set; } = new();
        public Dictionary<string, object> output { get; set; } = new();
    }

    public class TlClipEntityEvent : TimelineClip
    {
        public TlClipEntityEvent(TimelineClipData data)
            : base(data)
        {
        }

        public override void OnStart(TimelinePlayer player)
        {
            if (Data.UserData.TryGetValue("Entity", out var entitykey))
            {
                if (player.Blackboard.TryGetValue2(entitykey, out Entity entity))
                {
                    if (entity != null)
                    {
                        if (Data.UserData.TryGetValue("SelfEvent", out var sev))
                        {
                            var evt = entity.GenSelfEvent<TlSelfEvent>();
                            evt.input = new Dictionary<string, object>();
                            evt.input["BlackBoard"] = player.Blackboard;
                            evt.input["ClipData"] = Data.UserData;
                            evt.input["Msg"] = sev;
                            evt.Broadcast();
                            foreach (var kv in evt.output)
                            {
                                player.SetBlackboard(kv.Key, kv.Value);
                            }
                        }
                        if (Data.UserData.TryGetValue("Event", out var ev))
                        {
                            var evt = entity.GenEvent<TlEvent>();
                            evt.input = new Dictionary<string, object>();
                            evt.input["BlackBoard"] = player.Blackboard;
                            evt.input["ClipData"] = Data.UserData;
                            evt.input["Msg"] = ev;

                            evt.Broadcast();
                            foreach (var kv in evt.output)
                            {
                                player.SetBlackboard(kv.Key, kv.Value);
                            }
                        }
                    }
                }
            }
        }

        public override void OnEnd(TimelinePlayer player)
        {
        }

        public override void OnUpdate(TimelinePlayer timelinePlayer, float tm)
        {
        }
    }
}