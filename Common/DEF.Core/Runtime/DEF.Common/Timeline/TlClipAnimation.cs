using System.Collections.ObjectModel;

namespace DEF
{
    public class TlClipAnimation : TimelineClip
    {
        public TlClipAnimation(TimelineClipData data)
            : base(data)
        {
        }

        public override void OnStart(TimelinePlayer player)
        {
#if DEF_CLIENT
            if (Data.UserData.TryGetValue("Animator", out var animatorkey) && Data.UserData.TryGetValue("Animation", out var aniamtionname))
            {
                if (player.Blackboard.TryGetValue(animatorkey, out var v))
                {
                    var animator = (UnityEngine.Animator)v;
                    animator?.Play(aniamtionname);
                }
            }
            if (Data.UserData.TryGetValue("Animators", out var animatorskey) && Data.UserData.TryGetValue("Animation", out var aniamtionname2))
            {
                if (player.Blackboard.TryGetValue(animatorskey, out var v))
                {
                    var animators = (ReadOnlyCollection<UnityEngine.Animator>)v;
                    for (int i = 0; i < animators.Count; ++i)
                    {
                        animators[i]?.Play(aniamtionname2);
                    }
                }
            }
#endif
        }

        public override void OnEnd(TimelinePlayer player)
        {
        }

        public override void OnUpdate(TimelinePlayer timelinePlayer, float tm)
        {
        }
    }
}