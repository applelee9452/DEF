using System;
using System.Collections.Generic;

namespace DEF
{
    public abstract class SyncTagBase
    {
        protected SyncTagBase Parent { get; set; }

        public abstract SyncTagBase GetParent();

        public abstract List<SyncTagBase> GetChildren();
    }

    public static class TagManager
    {
        static Dictionary<int, SyncTagBase> MapAllTag { get; set; } = new();

        public static void Init(SyncTagBase tag)
        {
            if (tag == null) return;

            if (MapAllTag.ContainsKey(tag.GetHashCode()))
            {
                // todo，log error，TagId重复
            }

            MapAllTag[tag.GetHashCode()] = tag;

            var children = tag.GetChildren();
            if (children != null)
            {
                foreach (var i in children)
                {
                    Init(i);
                }
            }
        }

        public static SyncTagBase FindTag(int id)
        {
            MapAllTag.TryGetValue(id, out var tag);

            return tag;
        }
    }

    public class EntitySyncTag
    {
        Scene Scene { get; set; }
        Entity Entity { get; set; }
        Dictionary<int, byte[]> SyncTags { get; set; }// Value=UserData
        Dictionary<int, int> SyncTagsRefCount { get; set; }// Tag的引用计数
        Dictionary<int, Action<SyncTagBase>> MapAddSyncTagEvent { get; set; }
        Dictionary<int, Action<SyncTagBase>> MapRemoveSyncTagEvent { get; set; }
        Dictionary<int, Action<SyncTagBase>> MapValueChangedEvent { get; set; }

        public EntitySyncTag(Scene scene, Entity entity)
        {
            Scene = scene;
            Entity = entity;
        }

        public EntitySyncTag(Scene scene, Entity entity, Dictionary<int, byte[]> tags, Dictionary<int, int> tags_refcount)
        {
            Scene = scene;
            Entity = entity;
            SyncTags = tags;
            SyncTagsRefCount = tags_refcount;
        }

        public void Destroy()
        {
            if (SyncTags != null)
            {
                SyncTags.Clear();
                SyncTags = null;
            }

            if (SyncTagsRefCount != null)
            {
                SyncTagsRefCount.Clear();
                SyncTagsRefCount = null;
            }

            if (MapAddSyncTagEvent != null)
            {
                MapAddSyncTagEvent.Clear();
                MapAddSyncTagEvent = null;
            }

            if (MapRemoveSyncTagEvent != null)
            {
                MapRemoveSyncTagEvent.Clear();
                MapRemoveSyncTagEvent = null;
            }

            if (MapValueChangedEvent != null)
            {
                MapValueChangedEvent.Clear();
                MapValueChangedEvent = null;
            }

            Entity = null;
            Scene = null;
        }

        public bool ContainAllSyncTag(SyncTagBase tag)
        {
            if (SyncTags == null) return false;

            return SyncTags.ContainsKey(tag.GetHashCode());
        }

        public bool ContainNoneSyncTag(SyncTagBase tag)
        {
            if (SyncTags == null) return true;

            return !SyncTags.ContainsKey(tag.GetHashCode());
        }

        public void AddSyncTag(SyncTagBase tag)
        {
            if (tag == null) return;

            SyncTags ??= new Dictionary<int, byte[]>();
            SyncTagsRefCount ??= new Dictionary<int, int>();

            while (true)
            {
                int n = tag.GetHashCode();

                if (!SyncTags.ContainsKey(n))
                {
                    SyncTags.Add(n, null);
                    SyncTagsRefCount[n] = 1;

#if !DEF_CLIENT
                    if (Entity.EntityTagSyncFlag && Entity.NetworkSyncFlag)
                    {
                        Scene.WriteNetworkSyncBinlogAddEntityTag(Entity.ClientSubFilter, Entity.Id, n);
                    }
#endif

                    if (MapAddSyncTagEvent != null)
                    {
                        if (MapAddSyncTagEvent.TryGetValue(n, out var action))
                        {
                            action?.Invoke(tag);
                        }
                    }
                }
                else
                {
                    int count = SyncTagsRefCount[n];
                    count++;
                    SyncTagsRefCount[n] = count;
                }

                tag = tag.GetParent();
                if (tag == null) break;
            }
        }

        public void RemoveSyncTag(SyncTagBase tag)
        {
            if (tag == null) return;

            if (SyncTags == null) return;

            while (true)
            {
                int n = tag.GetHashCode();

                if (SyncTags.ContainsKey(n))
                {
                    int count = SyncTagsRefCount[n];
                    count--;
                    SyncTagsRefCount[n] = count;

                    if (count <= 0)
                    {
                        SyncTags.Remove(n);
                        SyncTagsRefCount.Remove(n);

#if !DEF_CLIENT
                        if (Entity.EntityTagSyncFlag && Entity.NetworkSyncFlag)
                        {
                            Scene.WriteNetworkSyncBinlogRemoveEntityTag(Entity.ClientSubFilter, Entity.Id, n);
                        }
#endif

                        if (MapRemoveSyncTagEvent != null)
                        {
                            if (MapRemoveSyncTagEvent.TryGetValue(n, out var action))
                            {
                                action?.Invoke(tag);
                            }
                        }
                    }
                }
                else
                {
                    SyncTagsRefCount.Remove(n);
                }

                tag = tag.GetParent();
                if (tag == null) break;
            }
        }

        public void SetSyncTagValue(SyncTagBase tag, byte[] v)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return;
            }

            int n = tag.GetHashCode();
            if (!SyncTags.TryGetValue(n, out var v2))
            {
                // todo，log error
                return;
            }

            v2 = v;
            SyncTags[n] = v2;

#if !DEF_CLIENT
            if (Entity.EntityTagSyncFlag && Entity.NetworkSyncFlag)
            {
                Scene.WriteNetworkSyncBinlogSetEntityTagValue(Entity.ClientSubFilter, Entity.Id, n, v2);
            }
#endif

            if (MapValueChangedEvent != null)
            {
                if (MapValueChangedEvent.TryGetValue(n, out var action))
                {
                    action?.Invoke(tag);
                }
            }
        }

        public void SetSyncTagValueAsInt(SyncTagBase tag, int v)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return;
            }

            int n = tag.GetHashCode();
            if (!SyncTags.TryGetValue(n, out var v2))
            {
                // todo，log error
                return;
            }

            v2 = BitConverter.GetBytes(v);
            SyncTags[n] = v2;

#if !DEF_CLIENT
            if (Entity.EntityTagSyncFlag && Entity.NetworkSyncFlag)
            {
                Scene.WriteNetworkSyncBinlogSetEntityTagValue(Entity.ClientSubFilter, Entity.Id, n, v2);
            }
#endif

            if (MapValueChangedEvent != null)
            {
                if (MapValueChangedEvent.TryGetValue(n, out var action))
                {
                    action?.Invoke(tag);
                }
            }
        }

        public int GetSyncTagValueAsInt(SyncTagBase tag)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return 0;
            }

            if (!SyncTags.TryGetValue(tag.GetHashCode(), out var v2))
            {
                // todo，log error
                return 0;
            }

            if (v2 == null)
            {
                // todo，log error
                return 0;
            }

            return BitConverter.ToInt32(v2, 0);
        }

        public void SetSyncTagValueAsLong(SyncTagBase tag, long v)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return;
            }

            int n = tag.GetHashCode();
            if (!SyncTags.TryGetValue(n, out var v2))
            {
                // todo，log error
                return;
            }

            v2 = BitConverter.GetBytes(v);
            SyncTags[n] = v2;

#if !DEF_CLIENT
            if (Entity.EntityTagSyncFlag && Entity.NetworkSyncFlag)
            {
                Scene.WriteNetworkSyncBinlogSetEntityTagValue(Entity.ClientSubFilter, Entity.Id, n, v2);
            }
#endif

            if (MapValueChangedEvent != null)
            {
                if (MapValueChangedEvent.TryGetValue(n, out var action))
                {
                    action?.Invoke(tag);
                }
            }
        }

        public long GetSyncTagValueAsLong(SyncTagBase tag)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return 0;
            }

            if (!SyncTags.TryGetValue(tag.GetHashCode(), out var v2))
            {
                // todo，log error
                return 0;
            }

            if (v2 == null)
            {
                // todo，log error
                return 0;
            }

            return BitConverter.ToInt64(v2, 0);
        }

        public void SetSyncTagValueAsString(SyncTagBase tag, string v)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return;
            }

            int n = tag.GetHashCode();
            if (!SyncTags.TryGetValue(n, out var v2))
            {
                // todo，log error
                return;
            }

            if (string.IsNullOrEmpty(v))
            {
                v2 = null;
            }
            else
            {
                v2 = System.Text.Encoding.UTF8.GetBytes(v);
            }

            SyncTags[n] = v2;

#if !DEF_CLIENT
            if (Entity.EntityTagSyncFlag && Entity.NetworkSyncFlag)
            {
                Scene.WriteNetworkSyncBinlogSetEntityTagValue(Entity.ClientSubFilter, Entity.Id, n, v2);
            }
#endif

            if (MapValueChangedEvent != null)
            {
                if (MapValueChangedEvent.TryGetValue(n, out var action))
                {
                    action?.Invoke(tag);
                }
            }
        }

        public string GetSyncTagValueAsString(SyncTagBase tag)
        {
            if (SyncTags == null)
            {
                // todo，log error
                return string.Empty;
            }

            if (!SyncTags.TryGetValue(tag.GetHashCode(), out var v2))
            {
                // todo，log error
                return string.Empty;
            }

            if (v2 == null || v2.Length == 0)
            {
                // todo，log error
                return string.Empty;
            }

            return System.Text.Encoding.UTF8.GetString(v2);
        }

        public void OnSyncTagEvent(SyncTagBase tag, Action<SyncTagBase> action_add, Action<SyncTagBase> action_remove, Action<SyncTagBase> action_valuechanged)
        {
            if (action_add != null)
            {
                MapAddSyncTagEvent ??= new();

                int n = tag.GetHashCode();

                MapAddSyncTagEvent[n] = action_add;
            }

            if (action_remove != null)
            {
                MapRemoveSyncTagEvent ??= new();

                int n = tag.GetHashCode();

                MapRemoveSyncTagEvent[n] = action_remove;
            }

            if (action_valuechanged != null)
            {
                MapValueChangedEvent ??= new();

                int n = tag.GetHashCode();

                MapValueChangedEvent[n] = action_valuechanged;
            }
        }

        public Dictionary<int, byte[]> GetSyncTags()
        {
            return SyncTags;
        }

        public Dictionary<int, int> GetSyncTagsRefCount()
        {
            return SyncTagsRefCount;
        }
    }
}