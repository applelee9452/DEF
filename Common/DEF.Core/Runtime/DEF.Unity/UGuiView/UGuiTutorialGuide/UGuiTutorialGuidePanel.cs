#if DEF_CLIENT

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UGuiTutorialGuidePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    public enum EventHandler
    {
        PointerClickHandler = 0,
        PointerUpHandler = 1,
        PointerDownHandler = 2,
        DragHandler = 3,
    }

    private UGuiTutorialGuideController guideController;
    private Canvas canvas;
    private RectTransform rect;
    private bool hasDestroy = false;

    private void OnDestroy()
    {
        hasDestroy = true;
        guideController = null;
        canvas = null;
        rect = null;
    }

    public void SetUIMask(Vector2 pos, Vector2 size, bool isRectMask = true)
    {
        hasDestroy = false;
        canvas = transform.GetComponentInParent<Canvas>();
        guideController = transform.GetComponent<UGuiTutorialGuideController>();
        rect = transform.Find("ImageMask").GetComponent<RectTransform>();
        if (isRectMask)
            SetRectMask(pos, size);
        else
            SetCircleMask(pos, size);
    }

    public void SetUIMask(RectTransform rect, bool isRectMask = true)
    {
        hasDestroy = false;
        if (isRectMask)
            guideController.Guide(canvas, rect, GuideType.Rect, TranslateType.Slow, 0.2f);
        else
            guideController.Guide(canvas, rect, GuideType.Circle, TranslateType.Slow, 0.2f);
    }

    private void SetRectMask(Vector2 pos, Vector2 size)
    {
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        guideController.Guide(canvas, rect, GuideType.Rect);
    }

    private void SetCircleMask(Vector2 pos, Vector2 size)
    {
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        guideController.Guide(canvas, rect, GuideType.Circle, TranslateType.Slow, 0.2f);
    }

#if UNITY_EDITOR
    // 用于编辑器下调整挖孔位置
    void Update()
    {
        //try
        //{
        //    if (Input.GetKeyUp(KeyCode.R))
        //    {
        //        guideController?.Guide(canvas, rect, GuideType.Rect);
        //    }
        //    if (Input.GetKeyUp(KeyCode.C))
        //    {
        //        guideController?.Guide(canvas, rect, GuideType.Circle);
        //    }
        //}
        //catch (System.Exception e)
        //{
        //    Debug.LogWarning($"此功能只能在编辑器下工作！：{e}");
        //}
    }
#endif

    private void SendEventDataToTouchCube(ExtendedPointerEventData data, EventHandler eventHandler)
    {
        if (hasDestroy) return;
        if (guideController == null)
            guideController = transform.GetComponent<UGuiTutorialGuideController>();

        if (guideController == null) return;
        //var b = guideController.IsRaycastLocationValid(new Vector2(Input.mousePosition.x, Input.mousePosition.y), Camera.main);
        //if (!b)
        //{
        //    List<RaycastResult> results = new List<RaycastResult>();
        //    EventSystem.current.RaycastAll(data, results);
        //    //m_Raycaster.Raycast(data, results);
        //    foreach (RaycastResult ray in results)
        //    {
        //        if (ray.gameObject.name.StartsWith("TouchCube"))
        //        {
        //            Debug.Log($"is In Rect:{Input.mousePosition.x}, {Input.mousePosition.y}");
        //            switch (eventHandler)
        //            {
        //                case EventHandler.PointerUpHandler:
        //                    ExecuteEvents.Execute(ray.gameObject, data, ExecuteEvents.pointerUpHandler);
        //                    break;
        //                case EventHandler.PointerDownHandler:
        //                    ExecuteEvents.Execute(ray.gameObject, data, ExecuteEvents.pointerDownHandler);
        //                    break;
        //                case EventHandler.PointerClickHandler:
        //                    ExecuteEvents.Execute(ray.gameObject, data, ExecuteEvents.pointerClickHandler);
        //                    break;
        //                case EventHandler.DragHandler:
        //                    ExecuteEvents.Execute(ray.gameObject, data, ExecuteEvents.dragHandler);
        //                    break;
        //            }
        //        }
        //    }
        //}
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        ExtendedPointerEventData data = eventData as ExtendedPointerEventData;
        SendEventDataToTouchCube(data, EventHandler.PointerDownHandler);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        ExtendedPointerEventData data = eventData as ExtendedPointerEventData;
        SendEventDataToTouchCube(data, EventHandler.PointerUpHandler);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        ExtendedPointerEventData data = eventData as ExtendedPointerEventData;
        SendEventDataToTouchCube(data, EventHandler.PointerClickHandler);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        ExtendedPointerEventData data = eventData as ExtendedPointerEventData;
        SendEventDataToTouchCube(data, EventHandler.DragHandler);
    }
}

#endif