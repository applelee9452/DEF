using System;
using System.Collections.Generic;

namespace DEF
{
    public class UpdateData
    {
        public bool SignRemove { get; set; } = false;
    }

    // 无序的Updater，需要和有序的Update配合使用。最好是嵌套在有序的Update流程中作为一个子函数被调用
    public class UpdateManager
    {
        Dictionary<System.Action<float>, UpdateData> MapUpdate { get; set; } = new();// Value=UpdateData，默认false，待移除变为true
        HashSet<System.Action<float>> HashSetSignRemove { get; set; } = new();
        List<System.Action<float>> ListSignAdd { get; set; } = new();
        bool InUpdate { get; set; }

        public void Update(float tm)
        {
            InUpdate = true;

            foreach (var item in MapUpdate)
            {
                if (!item.Value.SignRemove)
                {
                    item.Key(tm);
                }
            }

            InUpdate = false;

            foreach (var func in ListSignAdd)
            {
                UpdateData data = new()
                {
                    SignRemove = false
                };
                MapUpdate.Add(func, data);
            }
            ListSignAdd.Clear();

            foreach (var func in HashSetSignRemove)
            {
                MapUpdate.Remove(func);
            }
            HashSetSignRemove.Clear();
        }

        public void Add(Action<float> func)
        {
            if (InUpdate)
            {
                ListSignAdd.Add(func);
            }
            else
            {
                UpdateData data = new()
                {
                    SignRemove = false
                };

                MapUpdate[func] = data;
            }
        }

        public void Remove(Action<float> func)
        {
            if (InUpdate)
            {
                if (MapUpdate.TryGetValue(func, out var data))
                {
                    data.SignRemove = true;
                }

                HashSetSignRemove.Add(func);
            }
            else
            {
                MapUpdate.Remove(func);
            }
        }

        public void Delay(Action action, float time)
        {
            Action<float> action2 = null;
            action2 = (tm) =>
            {
                time -= tm;
                if (time > 0)
                {
                    return;
                }
                action();
                Remove(action2);
            };
            Add(action2);
        }
    }

    public sealed partial class Scene
    {
        public UpdateManager Updater { get; private set; } = new();
    }
}