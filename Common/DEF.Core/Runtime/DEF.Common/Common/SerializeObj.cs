#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;

namespace DEF
{
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj
    {
        public virtual object[] ToObjectArray()
        {
            return null;
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3, T4> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;
        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public T4 obj4;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3, obj4 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3, T4, T5> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;
        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public T4 obj4;
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public T5 obj5;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3, obj4, obj5 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3, T4, T5, T6> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;
        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public T4 obj4;
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public T5 obj5;
        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public T6 obj6;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3, obj4, obj5, obj6 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3, T4, T5, T6, T7> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;
        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public T4 obj4;
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public T5 obj5;
        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public T6 obj6;
        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public T7 obj7;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3, obj4, obj5, obj6, obj7 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3, T4, T5, T6, T7, T8> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;
        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public T4 obj4;
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public T5 obj5;
        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public T6 obj6;
        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public T7 obj7;
        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public T8 obj8;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8 };
        }
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SerializeObj<T1, T2, T3, T4, T5, T6, T7, T8, T9> : SerializeObj
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public T1 obj1;
        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public T2 obj2;
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public T3 obj3;
        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public T4 obj4;
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public T5 obj5;
        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public T6 obj6;
        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public T7 obj7;
        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public T8 obj8;
        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public T9 obj9;

        public override object[] ToObjectArray()
        {
            return new object[] { obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9 };
        }
    }
}