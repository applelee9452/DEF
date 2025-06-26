namespace DEF
{
    public interface IEbEvent
    {
        string name { get; set; }
    }

    public struct EbEvent0 : IEbEvent
    {
        public string name { get; set; }

        public EbEvent0(string event_name)
        {
            name = event_name;
        }
    };

    public struct EbEvent1<T> : IEbEvent
    {
        public string name { get; set; }
        public T param1;

        public EbEvent1(string event_name, T p1)
        {
            name = event_name;
            param1 = p1;
        }
    }

    public struct EbEvent2<T1, T2> : IEbEvent
    {
        public string name { get; set; }
        public T1 param1;
        public T2 param2;

        public EbEvent2(string event_name, T1 p1, T2 p2)
        {
            name = event_name;
            param1 = p1;
            param2 = p2;
        }
    }

    public struct EbEvent3<T1, T2, T3> : IEbEvent
    {
        public string name { get; set; }
        public T1 param1;
        public T2 param2;
        public T3 param3;

        public EbEvent3(string event_name, T1 p1, T2 p2, T3 p3)
        {
            name = event_name;
            param1 = p1;
            param2 = p2;
            param3 = p3;
        }
    }

    public struct EbEvent4<T1, T2, T3, T4> : IEbEvent
    {
        public string name { get; set; }
        public T1 param1;
        public T2 param2;
        public T3 param3;
        public T4 param4;

        public EbEvent4(string event_name, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            name = event_name;
            param1 = p1;
            param2 = p2;
            param3 = p3;
            param4 = p4;
        }
    }

    public struct EbEvent5<T1, T2, T3, T4, T5> : IEbEvent
    {
        public string name { get; set; }
        public T1 param1;
        public T2 param2;
        public T3 param3;
        public T4 param4;
        public T5 param5;

        public EbEvent5(string event_name, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            name = event_name;
            param1 = p1;
            param2 = p2;
            param3 = p3;
            param4 = p4;
            param5 = p5;
        }
    }
}