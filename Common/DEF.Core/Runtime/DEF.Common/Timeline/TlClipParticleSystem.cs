namespace DEF
{
    public class TlClipParticleSystem : TimelineClip
    {
        public TlClipParticleSystem(TimelineClipData data)
            : base(data)
        {
        }

        public override void OnStart(TimelinePlayer player)
        {
#if DEF_CLIENT
            if (Data.UserData.TryGetValue("PatticleSystem", out var key))
            {
                if (player.Blackboard.TryGetValue(key, out var v))
                {
                    var animator = (UnityEngine.ParticleSystem)v;
                    animator?.Play();
                }
            }
#endif
        }

        public override void OnEnd(TimelinePlayer player)
        {
#if DEF_CLIENT
            if (Data.UserData.TryGetValue("PatticleSystem", out var key))
            {
                if (player.Blackboard.TryGetValue(key, out var v))
                {
                    var animator = (UnityEngine.ParticleSystem)v;
                    animator?.Stop();
                }
            }
#endif
        }

        public override void OnUpdate(TimelinePlayer timelinePlayer, float tm)
        {
        }
    }
}