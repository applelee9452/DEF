#if DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientBehaviour : MonoBehaviour
{
    private void OnApplicationPause(bool pause)
    {
        Client.OnApplicationPause(pause);
    }

    private void OnApplicationFocus(bool focus_status)
    {
        Client.OnApplicationFocus(focus_status);
    }
}

#endif