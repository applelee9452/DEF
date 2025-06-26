#if DEF_CLIENT
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiButtonEffectScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Header("����ʱ��С�����٣�")]
    private float DownScale = 1.1f;
    [SerializeField, Header("���ű仯����ʱ�䣺���¹���")]
    private float DownDuration = 0.2f;
    [SerializeField, Header("���ű仯����ʱ�䣺̧�����")]
    private float UpDuration = 0.15f;

    private Tween ScaleTween;

    private RectTransform _rectTransform;

    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    private void OnDisable()
    {
        ReleaseScaleTween();
        RectTransform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        ReleaseScaleTween();
        RectTransform.localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ReleaseScaleTween();
        ScaleTween = RectTransform.DOScale(DownScale, DownDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReleaseScaleTween();
        ScaleTween = RectTransform.DOScale(1, UpDuration);
    }

    private void ReleaseScaleTween()
    {
        if (ScaleTween != null)
        {
            ScaleTween.Kill();
            ScaleTween = null;
        }
    }

}

#endif