namespace DEF
{
    public class TlClipAudio : TimelineClip
    {    
        public TlClipAudio(TimelineClipData data)
            : base(data)
        {
        }

        public override void OnStart(TimelinePlayer player)
        {
#if DEF_CLIENT
            if (Data.UserData.TryGetValue("Audio", out var audiokey))
            {
                if (player.Blackboard.TryGetValue(audiokey, out var v))
                {
                    var audio = (UnityEngine.AudioSource)v;
                    audio.Play();
                }
            }
#endif
        }

        public override void OnEnd(TimelinePlayer player)
        {
#if DEF_CLIENT
            if (Data.UserData.TryGetValue("Audio", out var audiokey))
            {
                if (player.Blackboard.TryGetValue(audiokey, out var v))
                {
                    var audio = (UnityEngine.AudioSource)v;
                    audio.Stop();
                }
            }
#endif
        }

        public override void OnUpdate(TimelinePlayer timelinePlayer, float tm)
        {
        }
    }
}