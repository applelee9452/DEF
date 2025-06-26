using System;

namespace DEF
{
    public class FixRandom
    {
        public Fix64 One()
        {
            return Next() / (Fix64)M;
        }

        public int Range(int maxValue)
        {
            return (int)(One() * maxValue);
        }

        public int Range(int minValue, int maxValue)
        {
            var range = maxValue - minValue;
            return (int)(Range(range) + minValue);
        }

        public Fix64 Range(Fix64 maxValue)
        {
            return One() * maxValue;
        }

        public Fix64 Range(Fix64 minValue, Fix64 maxValue)
        {
            var range = maxValue - minValue;
            return Range(range) + minValue;
        }

        public FixRandom CreateRandom()
        {
            return new FixRandom(Next());
        }

        public int Seed { set; get; }
        public FixRandom()
        {
            this.Seed = new System.Random().Next();
        }

        public FixRandom(int seed)
        {
            this.Seed = seed;
        }

        const long M = int.MaxValue;
        const long A = 48271;
        const long B = 0;
        private int Next()
        {
            Seed = (int)((A * Seed + B) % M);
            return Seed;
        }
    }
}