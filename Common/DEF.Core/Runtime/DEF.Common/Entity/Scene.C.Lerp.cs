#if DEF_CLIENT

using System.Collections.Generic;

namespace DEF
{
    public sealed partial class Scene
    {
        HashSet<ISyncLerp> HashSetSyncLerp { get; set; }

        public void AddLerp(ISyncLerp lerp)
        {
            HashSetSyncLerp ??= new HashSet<ISyncLerp>();

            HashSetSyncLerp.Add(lerp);
        }

        public void RemoveLerp(ISyncLerp lerp)
        {
            if (HashSetSyncLerp == null) return;

            HashSetSyncLerp.Remove(lerp);
        }

        public void UpdateLerp(float tm)
        {
            if (HashSetSyncLerp == null) return;

            foreach (var lerp in HashSetSyncLerp)
            {
                lerp.Update(tm);
            }
        }
    }
}

#endif