using System;
using System.Collections.Generic;

namespace DEF
{
    public static class TimelineFactory
    {
        private static Dictionary<string, Func<TimelineClipData, TimelineClip>> MapFactory { get; set; } = new();

        static TimelineFactory()
        {
            Register("Empty", (d) => new TlClipEmpty(d));
            Register("Animation", (d) => new TlClipAnimation(d));
            Register("Audio", (d) => new TlClipAudio(d));
            Register("EntityEvent", (d) => new TlClipEntityEvent(d));
            Register("ParticleSystem", (d) => new TlClipParticleSystem(d));
        }

        public static void Register(string key, Func<TimelineClipData, TimelineClip> factory)
        {
            MapFactory[key] = factory;
        }

        public static TimelineClip CreateClip(TimelineClipData i)
        {
            if (MapFactory.TryGetValue(i.ClipType, out var v))
            {
                return v(i);
            }
            return null;
        }
    }
}