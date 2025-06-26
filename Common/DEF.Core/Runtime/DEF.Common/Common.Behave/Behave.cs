using System.Collections.Generic;

namespace NPBehave
{
    public class Behave
    {
        public System.Random Rd { get; private set; } = new();
        public Clock Clock { get; private set; }

        private Dictionary<string, Blackboard> blackboards;

        public Behave()
        {
            blackboards = new();
            Clock = new(this);
        }

        public Blackboard GetSharedBlackboard(string key)
        {
            if (!blackboards.ContainsKey(key))
            {
                blackboards.Add(key, new Blackboard(Clock));
            }

            return blackboards[key];
        }

        public void Update(float tm)
        {
            Clock.Update(tm);
        }
    }
}