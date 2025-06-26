using System.Collections.Generic;
using System;

namespace DEF
{
    public interface IComponentHook<T> where T : Component
    {
        public T Com { get; set; }

        void OnAwake(Dictionary<string, object> create_params);

        void OnStart();

        void OnDestroy(string reason, byte[] user_data);

        void HandleEvent(DEF.Event ev);
    }

    public interface IComponentHookFactory
    {
    }

    public interface IComponentHookFactory<T> : IComponentHookFactory where T : Component
    {
        IComponentHook<T> CreateHook(T com);
    }
}