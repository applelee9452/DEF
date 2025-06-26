#if DEF_CLIENT

//#if UNITY_EDITOR
//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
//#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DEF
{
    //#if UNITY_EDITOR
    //    public sealed partial class Entity : SerializedScriptableObject
    //#else
    //    public sealed partial class Entity
    //#endif
    public sealed partial class Entity
    {
        void DestroyClient()
        {
        }
    }
}

#endif