namespace DEF
{
    public enum SerializerType
    {
        LitJson = 0,
        Protobuf,
        MemoryPack,
    }

    public enum ContainerStateType
    {
        Stateless = 0,
        Stateful,
        StatelessNoReentrant,
        StatefulNoReentrant,
    }

    // 性别
    public enum GenderType
    {
        Unknow = 0,// 位置
        Male,// 男
        Female,// 女
        MaleFromFemale,// 女变男
        FemaleFromMale,// 男变女
    }

    public delegate void OnPropChanged<T>(T v);
}