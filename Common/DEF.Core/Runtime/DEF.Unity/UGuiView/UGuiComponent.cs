#if DEF_CLIENT

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UGuiComponent : MonoBehaviour
{
    public object UserData;

    [SerializeField]
    private string ScriptObjName;

    [SerializeField]
    List<UGuiGo> Objs;

    UGuiView UGuiView { get; set; }

    void Start()
    {
        UGuiView = GetComponent<UGuiView>();
        if (UGuiView == null)
        {
            UGuiView = GetComponentInParent<UGuiView>();
        }

        if (!string.IsNullOrEmpty(ScriptObjName))
        {
            if (UGuiView != null)
            {
                var wrapper = Client.AssemblesWrapper;
                wrapper?.OnUGuiCreateCom(UGuiView.gameObject, gameObject, ScriptObjName);
            }
        }

        if(gameObject.TryGetComponent<Button>(out var button))
        {
            button.transition = Selectable.Transition.None;
            var effect_scale = GetComponent<UiButtonEffectScale>();
            if(effect_scale == null)
            {
                gameObject.AddComponent<UiButtonEffectScale>();
            }
        }
    }

    void GetUGuiView()
    {
        UGuiView = GetComponent<UGuiView>();
        if (UGuiView == null)
        {
            UGuiView = GetComponentInParent<UGuiView>();
        }
    }

    public void OnMove(BaseEventData data)
    {
        if (UGuiView == null)
        {
            GetUGuiView();
        }

        var wrapper = Client.AssemblesWrapper;
        wrapper?.OnUGuiMove(UGuiView.gameObject.name, gameObject, data);
    }

    public void OnButtonClick()
    {
        if (UGuiView == null)
        {
            GetUGuiView();
        }

        var wrapper = Client.AssemblesWrapper;
        wrapper?.OnUGuiButtonClick(UGuiView.gameObject.name, gameObject.name, gameObject);
    }

    public void OnSliderValueChanged(float v)
    {
        if (UGuiView == null)
        {
            UGuiView = GetComponent<UGuiView>();
        }

        if (UGuiView == null)
        {
            UGuiView = GetComponentInParent<UGuiView>();
        }

        var wrapper = Client.AssemblesWrapper;
        wrapper?.OnUGuiSliderValueChange(UGuiView.gameObject.name, gameObject.name, v);
    }

    public void OnSliderValueYChanged(Vector2 vect2)
    {
        if (UGuiView == null)
        {
            UGuiView = GetComponent<UGuiView>();
        }

        if (UGuiView == null)
        {
            UGuiView = GetComponentInParent<UGuiView>();
        }

        var wrapper = Client.AssemblesWrapper;
        wrapper?.OnUGuiSliderValueChange(UGuiView.gameObject.name, gameObject.name, vect2.y);
    }

    public T GetObj<T>(string key) where T : UnityEngine.Object
    {
        foreach (var obj in Objs)
        {
            if (obj.Key == key)
            {
                return typeof(T) == typeof(GameObject) ? (T)obj.Obj : ((GameObject)obj.Obj).GetComponent<T>();
            }
        }

        return default;
    }

    public Transform GetObjAsTransform(string key)
    {
        return GetObj<Transform>(key);
    }

    public TextMeshProUGUI GetObjAsTextMeshProUGUI(string key)
    {
        return GetObj<TextMeshProUGUI>(key);
    }

    public Text GetObjAsText(string key)
    {
        return GetObj<Text>(key);
    }

    public Image GetObjAsImage(string key)
    {
        return GetObj<Image>(key);
    }

    public Slider GetObjAsSlider(string key)
    {
        return GetObj<Slider>(key);
    }

    public Button GetObjAsButton(string key)
    {
        return GetObj<Button>(key);
    }

    public ScrollRect GetObjAsScrollRect(string key)
    {
        return GetObj<ScrollRect>(key);
    }

    public TMP_InputField GetObjAsTmpInputField(string key)
    {
        return GetObj<TMP_InputField>(key);
    }
}

#endif