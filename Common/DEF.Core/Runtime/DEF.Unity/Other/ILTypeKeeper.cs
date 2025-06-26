#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Linq;

public class ILTypeKeeper
{
    public List<Type> ListType = new()
    {
        //typeof(ProtoBuf.Serializers.MapDecorator<Dictionary<int, int>, int, int>),
        //typeof(ProtoBuf.Serializers.MapDecorator<Dictionary<int, ulong>, int, ulong>),
        //typeof(ProtoBuf.Serializers.MapDecorator<Dictionary<int, string>, int, string>),
        //typeof(ProtoBuf.Serializers.MapDecorator<Dictionary<string, string>, string, string>),
    };

    public ILTypeKeeper()
    {
        {
            List<string> l1 = new();
            List<string> l2 = new();
            l1.Except(l2);

            {
                Dictionary<string, string> d1 = new();
                Dictionary<string, short> d2 = new();
                Dictionary<string, int> d3 = new();
                Dictionary<string, long> d4 = new();
                Dictionary<string, float> d5 = new();
                Dictionary<string, double> d6 = new();
                Dictionary<string, byte> d7 = new();
            }

            {
                Dictionary<short, string> d1 = new();
                Dictionary<short, short> d2 = new();
                Dictionary<short, int> d3 = new();
                Dictionary<short, long> d4 = new();
                Dictionary<short, float> d5 = new();
                Dictionary<short, double> d6 = new();
                Dictionary<short, byte> d7 = new();
            }

            {
                Dictionary<ushort, string> d1 = new();
                Dictionary<ushort, short> d2 = new();
                Dictionary<ushort, int> d3 = new();
                Dictionary<ushort, long> d4 = new();
                Dictionary<ushort, float> d5 = new();
                Dictionary<ushort, double> d6 = new();
                Dictionary<ushort, byte> d7 = new();
            }

            {
                Dictionary<int, string> d1 = new();
                Dictionary<int, short> d2 = new();
                Dictionary<int, int> d3 = new();
                Dictionary<int, long> d4 = new();
                Dictionary<int, float> d5 = new();
                Dictionary<int, double> d6 = new();
                Dictionary<int, byte> d7 = new();
            }

            {
                Dictionary<long, string> d1 = new();
                Dictionary<long, short> d2 = new();
                Dictionary<long, int> d3 = new();
                Dictionary<long, long> d4 = new();
                Dictionary<long, float> d5 = new();
                Dictionary<long, double> d6 = new();
                Dictionary<long, byte> d7 = new();
            }

            {
                Dictionary<float, string> d1 = new();
                Dictionary<float, short> d2 = new();
                Dictionary<float, int> d3 = new();
                Dictionary<float, long> d4 = new();
                Dictionary<float, float> d5 = new();
                Dictionary<float, double> d6 = new();
                Dictionary<float, byte> d7 = new();
            }

            {
                Dictionary<double, string> d1 = new();
                Dictionary<double, short> d2 = new();
                Dictionary<double, int> d3 = new();
                Dictionary<double, long> d4 = new();
                Dictionary<double, float> d5 = new();
                Dictionary<double, double> d6 = new();
                Dictionary<double, byte> d7 = new();
            }

            {
                Dictionary<byte, string> d1 = new();
                Dictionary<byte, short> d2 = new();
                Dictionary<byte, int> d3 = new();
                Dictionary<byte, long> d4 = new();
                Dictionary<byte, float> d5 = new();
                Dictionary<byte, double> d6 = new();
                Dictionary<byte, byte> d7 = new();
            }

            {
                List<string> d1 = new();
                List<short> d2 = new();
                List<int> d3 = new();
                List<long> d4 = new();
                List<float> d5 = new();
                List<double> d6 = new();
                List<byte> d7 = new();
            }

            {
                HashSet<string> d1 = new();
                HashSet<short> d2 = new();
                HashSet<int> d3 = new();
                HashSet<long> d4 = new();
                HashSet<float> d5 = new();
                HashSet<double> d6 = new();
                HashSet<byte> d7 = new();
            }

            {
                Queue<string> d1 = new();
                Queue<short> d2 = new();
                Queue<ushort> d3 = new();
                Queue<int> d4 = new();
                Queue<uint> d5 = new();
                Queue<long> d6 = new();
                Queue<float> d7 = new();
                Queue<double> d8 = new();
                Queue<byte> d9 = new();
            }
        }
    }
}

#endif