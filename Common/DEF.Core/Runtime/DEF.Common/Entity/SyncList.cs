using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DEF
{
    [Serializable]
    [ProtoContract]
    public class SyncList<T>
    {
        enum OpId:byte
        {
            None = 0,
            Add,
            AddRange
        }

        [ProtoIgnore]
        Scene Scene { get; set; }

        [ProtoIgnore]
        Component Component { get; set; }

        [ProtoIgnore]
        string StateName { get; set; }

        [ProtoMember(1)]
        List<T> ListItem { get; set; } = new();

        public void ApplyDirtyCustomState(byte cmd, byte[] value)
        {
            OpId op_id = (OpId)cmd;
            switch (op_id)
            {
                case OpId.Add:
                    {

                    }
                    break;
                case OpId.AddRange:
                    {

                    }
                    break;
            }

            if (cmd == 1)
            {
                // AddModify

                //var modify = MemoryPackSerializer.Deserialize<SyncVarModify>(value);

                //AddModify(modify.Key, modify.Op, modify.Modify);
            }
            else if (cmd == 2)
            {
                // RemoveModiy

                //string key = MemoryPackSerializer.Deserialize<string>(value);

                //RemoveModify(key);
            }
            else if (cmd == 3)
            {
                // SetDefaultValue

                //float base_value = MemoryPackSerializer.Deserialize<float>(value);

                //BaseValue = base_value;
                //CurrentValue = BaseValue;
            }
        }

        public void Init(Component component, string state_name)
        {
            Component = component;
            Scene = Component.Entity.Scene;
            StateName = state_name;
        }

        public void Release()
        {
        }

        public void Add(T item)
        {
            ListItem.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            ListItem.AddRange(collection);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
        {
            return ListItem.BinarySearch(index, count, item, comparer);
        }

        public int BinarySearch(T item)
        {
            return ListItem.BinarySearch(item);
        }

        public int BinarySearch(T item, IComparer<T>? comparer)
        {
            return ListItem.BinarySearch(item, comparer);
        }

        public void Clear()
        {
            ListItem.Clear();
        }

        public bool Contains(T item)
        {
            return ListItem.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ListItem.CopyTo(array, arrayIndex);
        }

        public void CopyTo(T[] array)
        {
            ListItem.CopyTo(array);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            ListItem.CopyTo(index, array, arrayIndex, count);
        }

        public bool Exists(Predicate<T> match)
        {
            return ListItem.Exists(match);
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return ListItem.FindAll(match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return ListItem.FindIndex(startIndex, count, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return ListItem.FindIndex(startIndex, match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return ListItem.FindIndex(match);
        }

        public T? FindLast(Predicate<T> match)
        {
            return ListItem.FindLast(match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return ListItem.FindLastIndex(startIndex, count, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return ListItem.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return ListItem.FindIndex(match);
        }

        public void ForEach(Action<T> action)
        {
            ListItem.ForEach(action);
        }

        public List<T> GetRange(int index, int count)
        {
            return ListItem.GetRange(index, count);
        }

        public int IndexOf(T item, int index, int count)
        {
            return ListItem.IndexOf(item, index, count);
        }

        public int IndexOf(T item, int index)
        {
            return ListItem.IndexOf(item, index);
        }

        public int IndexOf(T item)
        {
            return ListItem.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ListItem.Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            ListItem.InsertRange(index, collection);
        }

        public int LastIndexOf(T item)
        {
            return ListItem.LastIndexOf(item);
        }

        public int LastIndexOf(T item, int index)
        {
            return ListItem.LastIndexOf(item, index);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            return ListItem.LastIndexOf(item, index, count);
        }

        public bool Remove(T item)
        {
            return ListItem.Remove(item);
        }

        public int RemoveAll(Predicate<T> match)
        {
            return ListItem.RemoveAll(match);
        }

        public void RemoveAt(int index)
        {
            ListItem.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            ListItem.RemoveRange(index, count);
        }

        public void Reverse(int index, int count)
        {
            ListItem.Reverse(index, count);
        }

        public void Reverse()
        {
            ListItem.Reverse();
        }

        public void Sort(IComparer<T>? comparer)
        {
            ListItem.Sort(comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            ListItem.Sort(comparison);
        }

        public void Sort(int index, int count, IComparer<T>? comparer)
        {
            ListItem.Sort(index, count, comparer);
        }

        public void Sort()
        {
            ListItem.Sort();
        }

        public T[] ToArray()
        {
            return ListItem.ToArray();
        }

        public void TrimExcess()
        {
            ListItem.TrimExcess();
        }

        public bool TrueForAll(Predicate<T> match)
        {
            return ListItem.TrueForAll(match);
        }
    }
}