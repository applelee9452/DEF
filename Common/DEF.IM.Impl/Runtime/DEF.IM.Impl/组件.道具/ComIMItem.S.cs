#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

public partial class ComIMItem
{
    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        if (create_params != null)
        {
            State.ItemId = (int)create_params["ItemId"];
            State.ItemObjId = (string)create_params["ItemObjId"];
        }
    }

    void OnStartServer()
    {
    }

    void OnDestroyServer(string reason, byte[] user_data)
    {
    }

    void HandleEventServer(DEF.Event ev)
    {
    }
}

#endif