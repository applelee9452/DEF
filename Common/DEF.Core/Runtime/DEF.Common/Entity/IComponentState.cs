namespace DEF
{
    public interface IComponentState
    {
#if DEF_CLIENT
        void ApplyDirtyState(string key, byte[] value);

        void ApplyDirtyCustomState(string key, byte cmd, byte[] value);
#endif

        void Release();
    }

    public abstract class ComponentStateFactory
    {
        public abstract string GetName();

        public abstract string GetName2();

        public abstract IComponentState CreateState(Component com, EntityStateSourceType source_type, object source);
    }
}