namespace DEF
{
    public class TlClipEmpty : TimelineClip
    {
        public TlClipEmpty(TimelineClipData data)
            : base(data)
        {
        }

        public override void OnStart(TimelinePlayer player)
        {
        }

        public override void OnEnd(TimelinePlayer player)
        {
        }

        public override void OnUpdate(TimelinePlayer timelinePlayer, float tm)
        {
        }
    }
}