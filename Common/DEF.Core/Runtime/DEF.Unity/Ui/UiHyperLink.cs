#if DEF_CLIENT

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiHyperLink : MonoBehaviour,IPointerClickHandler
{
    public Action<string> Callback { get; set; } = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        int linkIndex =TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position,Camera.main); 

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            Debug.Log($"UiHyperLink={linkInfo.GetLinkID()}");
            Callback?.Invoke(linkInfo.GetLinkID());
        }
    }   
}

#endif