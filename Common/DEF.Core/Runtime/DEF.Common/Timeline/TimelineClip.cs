namespace DEF
{
    public abstract class TimelineClip
    {
        public TimelineClipData Data { get; private set; }

        public TimelineClip(TimelineClipData data)
        {
            Data = data;
        }

        public abstract void OnStart(TimelinePlayer player);

        public abstract void OnEnd(TimelinePlayer player);

        public abstract void OnUpdate(TimelinePlayer timelinePlayer, float tm);
    }
}