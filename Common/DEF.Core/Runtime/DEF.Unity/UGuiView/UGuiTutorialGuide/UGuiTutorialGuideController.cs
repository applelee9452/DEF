#if DEF_CLIENT

using UnityEngine;
using UnityEngine.UI;

public enum GuideType
{
    Rect,
    Circle
}

[RequireComponent(typeof(UGuiTutorialCircleGuide))]
[RequireComponent(typeof(UGuiTutorialRectGuide))]
public class UGuiTutorialGuideController : MonoBehaviour, ICanvasRaycastFilter
{
    private UGuiTutorialCircleGuide circleGuide;
    private UGuiTutorialRectGuide rectGuide;

    public Material rectMat;
    public Material circleMat;

    private Image mask;

    private RectTransform target;

    private GuideType guideType;

    public Vector3 Center
    {
        get
        {

            switch (this.guideType)
            {
                case GuideType.Rect:
                    return rectGuide.Center;
                case GuideType.Circle:
                    return circleGuide.Center;
            }

            return rectGuide.Center;
        }
    }

    private void Awake()
    {
        mask = transform.GetComponent<Image>();

        if (mask == null) { throw new System.Exception("mask 初始化失败!"); }

        if (rectMat == null || circleMat == null) { throw new System.Exception("材质未赋值!"); }

        circleGuide = transform.GetComponent<UGuiTutorialCircleGuide>();
        rectGuide = transform.GetComponent<UGuiTutorialRectGuide>();
    }

    private void Guide(RectTransform target, GuideType guideType)
    {
        this.target = target;
        this.guideType = guideType;

        switch (guideType)
        {
            case GuideType.Rect:
                mask.material = rectMat;
                break;
            case GuideType.Circle:
                mask.material = circleMat;
                break;
        }
    }

    public void Guide(Canvas canvas, RectTransform target, GuideType guideType, TranslateType translateType = TranslateType.Direct, float time = 1)
    {
        Guide(target, guideType);

        switch (guideType)
        {
            case GuideType.Rect:
                rectGuide.Guide(canvas, target, translateType, time);
                break;
            case GuideType.Circle:
                circleGuide.Guide(canvas, target, translateType, time);
                break;
        }
    }

    public void Guide(Canvas canvas, RectTransform target, GuideType guideType, float scale, float time, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        Guide(target, guideType);

        switch (guideType)
        {
            case GuideType.Rect:
                rectGuide.Guide(canvas, target, scale, time, translateType, moveTime);
                break;
            case GuideType.Circle:
                circleGuide.Guide(canvas, target, scale, time, translateType, moveTime);
                break;
        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (target == null) { return true; } // 事件不会渗透
        //Vector3 size = new Vector3(target.offsetMax.x - target.offsetMin.x, target.offsetMax.y - target.offsetMin.y);
        //Bounds bounds = new Bounds(new Vector3(target.offsetMin.x + size.x / 2, target.offsetMin.y + size.y / 2), size);
        //bool b = bounds.Contains(sp);
        return !RectTransformUtility.RectangleContainsScreenPoint(target, sp, eventCamera);
    }
}

#endif