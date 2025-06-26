using System;
using System.Collections.Generic;

namespace DEF
{
    public struct EntityDef
    {
        public string Name;
        public Type[] Components;
        public List<EntityDef> Children;

        public EntityDef(string name, params Type[] components)
        {
            Name = name;
            Components = components;
            Children = null;
        }
    }

    public struct StateInfo
    {
        public string StateName { get; set; }
        public IComponentState State { get; set; }
    }

    public struct EntityDef4Bson
    {
        public long EntityId { get; set; }
        public string EntityName { get; set; }
        public List<StateInfo> States { get; set; }
        public List<EntityDef4Bson> Children { get; set; }
    }
}