#if DEF_CLIENT

using UnityEngine;

[ExecuteInEditMode]
public class UiHeightFitter : MonoBehaviour
{
    [SerializeField]
    private RectTransform Target;
    [SerializeField]
    private float Offset;
    [SerializeField]
    private float Min;
    private RectTransform Rect;

    private void Awake()
    {
        Rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Target==null||Rect==null)
        {
            return;
        }
        var y = Target.sizeDelta.y + Offset;
        var size = Rect.sizeDelta;
        if(y<Min)
        {
            y = Min;
        }
        Rect.sizeDelta=new Vector2(size.x, y);
    }
}

#endif