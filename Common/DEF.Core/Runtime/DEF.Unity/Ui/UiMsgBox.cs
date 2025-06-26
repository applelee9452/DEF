#if DEF_CLIENT

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiMsgBox : MonoBehaviour
{
    public TextMeshProUGUI TextInfo;
    public Button BtnOk;
    public Button BtnCancel;

    public void Show(string info, System.Action cb_ok, System.Action cb_cancel)
    {
        TextInfo.text = info;

        BtnOk.onClick.AddListener(() =>
        {
            cb_ok?.Invoke();
        });

        BtnCancel.onClick.AddListener(() =>
        {
            cb_cancel?.Invoke();
        });
    }
}

#endif