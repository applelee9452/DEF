#if DEF_CLIENT

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DEF.Client
{
    public enum UiLayer
    {
        Background = 0,
        Main,
        Popup1,
        Loading,
        Popup2,
        Top,
    }

    public enum ShowAnimType
    {
        PopupCenter = 0,
        PopupTop,
        PopupLeft,
    }

    public class EvRedPoint : DEF.Event
    {
        public string RedPointId { get; set; }
        public bool IsOn { get; set; }
    }

    // 一个View对应一个UI Prefab
    public abstract class View : EventListener
    {
        public GameObject Go { get; set; }
        public UGuiView UGuiView { get; set; }
        public Dictionary<string, string> DicLanguage { get; set; }
        public ViewFactory Factory { get; set; }
        public bool IsValid { get; set; }
        TweenerCore<Vector3, Vector3, VectorOptions> TweenClose { get; set; }
        TweenerCore<Vector3, Vector3, VectorOptions> TweenOpen { get; set; }
        Dictionary<string, UGuiRedPoint> MapRedPoint { get; set; }

        public abstract void OnCreate(object obj, Dictionary<string, string> create_params);

        public abstract void OnDestory();

        public virtual void OnUGuiCreateCom(GameObject view, GameObject obj, string script_com_name)
        {
        }

        public virtual void OnUGuiMove(GameObject go, BaseEventData data)
        {
        }

        public virtual void OnUGuiButtonClick(string button_name, GameObject go_button)
        {
        }

        public virtual void OnUGuiSliderValueChange(string slider_name, float value)
        {
        }

        public virtual void OnUGuiMsg(GameObject go, string msg)
        {
        }

        protected virtual Transform GetAnimationTransform()
        {
            return null;
        }

        protected virtual bool EnableOpenAnimation()
        {
            return false;
        }

        protected virtual bool EnableCloseAnimation()
        {
            return false;
        }

        protected virtual void OnPlayOpenAnimation()
        {
            var root = GetAnimationTransform();
            if (root == null)
            {
                return;
            }
            root.localScale = Vector3.zero;
            TweenOpen = root.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        protected virtual void OnPlayCloseAnimation(Action complete)
        {
            var root = GetAnimationTransform();
            if (root == null)
            {
                complete?.Invoke();
                return;
            }
            TweenClose = root
                .DOScale(Vector3.zero * 0.3f, 0.2f)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    complete?.Invoke();
                });
        }

        protected virtual void OnPlayOpenSound()
        {
            //目前没有通用的Ui打开音效
            //SoundMgr.Play("Assets/Arts/Sound/SfxUiOpen.wav", SoundLayer.LayerReplace);
        }

        public void Create(object obj, Dictionary<string, string> create_params)
        {
            OnCreate(obj, create_params);

            // 打开音效
            if (!Factory.DisableOpenAndCloseAudio())
            {
                OnPlayOpenSound();
            }

            // 渐入效果
            if (EnableOpenAnimation())
            {
                OnPlayOpenAnimation();
            }
        }

        public void Destory()
        {
            if (TweenOpen != null)
            {
                TweenOpen.Kill();
                TweenOpen = null;
            }

            if (TweenClose != null)
            {
                TweenClose.Kill();
                TweenClose = null;
            }

            OnDestory();
        }

        public void Close()
        {
            if (Go == null) return;

            // 关闭音效
            //if (!Factory.DisableOpenAndCloseAudio())
            //{
            //    SoundMgr.Play("Assets/Arts/Sound/SfxUiClose.wav", SoundLayer.LayerReplace);
            //}

            // 消失效果
            if (EnableCloseAnimation())
            {
                OnPlayCloseAnimation(() =>
                {
                    ViewMgr.DestroyView(this);
                });
            }
            else
            {
                ViewMgr.DestroyView(this);
            }
        }

        public void ShowAnim(RectTransform rtf, ShowAnimType show_anim_type = ShowAnimType.PopupCenter)
        {
            Go.SetActive(true);

            if (show_anim_type == ShowAnimType.PopupCenter)
            {
                rtf.localScale = new Vector3(0.3f, 0.3f, 1f);
                //TweenShowAnim1 = rtf.DOScale(new Vector3(1f, 1f, 1f), 0.15f);
            }
            else if (show_anim_type == ShowAnimType.PopupLeft)
            {
                rtf.localScale = new Vector3(0.3f, 0.3f, 1f);
                //TweenShowAnim1 = rtf.DOLocalMoveX(100f, 0.15f);
            }
            else if (show_anim_type == ShowAnimType.PopupTop)
            {
                rtf.localScale = new Vector3(0.3f, 0.3f, 1f);
                //TweenShowAnim1 = rtf.DOLocalMoveY(-100, 0.15f);
            }
        }

        public bool GetVisible()
        {
            return Go.activeSelf;
        }

        public void SetVisible(bool visible)
        {
            Go.SetActive(visible);
        }

        public void SetEventContext(EventContext event_context)
        {
            EventContext = event_context;
        }

        public void ListenEvent<T>() where T : Event
        {
            EventContext.ListenEvent<T>(this);
        }

        public void UnListenAllEvent()
        {
            EventContext.UnListenAllEvent(this);
        }

        public T GenEvent<T>() where T : Event, new()
        {
            return EventContext.GenEvent<T>();
        }

        public string GetLanguageString(string lan_key)
        {
            string lan = string.Empty;
            if (DicLanguage != null)
            {
                DicLanguage.TryGetValue(lan_key, out lan);
            }

            if (string.IsNullOrEmpty(lan))
            {
                Debug.LogWarning($"多语言Key不存在！Key={lan_key}");
                lan = lan_key;
            }

            return lan;
        }

        public void LoadTexture(string url, System.Action<string, Texture> cb)
        {
            //Context.Instance.UGuiTextureLoader.Load(url, cb);
        }

        public void MakeBgFitScreen(bool is_portrait, RectTransform rt, Vector2 textureOriginSize, RawImage img)
        {
            float screenxyRate = (float)Screen.width / Screen.height;// 当前画布尺寸长宽比
            float texturexyRate = textureOriginSize.x / textureOriginSize.y;// 视频尺寸长宽比

            //Debug.Log($"screenxyRate={screenxyRate} ScreenX={Screen.width} ScreenY={Screen.height}");
            //Debug.Log($"texturexyRate={texturexyRate} TextureX={textureOriginSize.x} TextureY={textureOriginSize.y}");

            if (is_portrait)
            {
                // 竖屏

                if (texturexyRate > screenxyRate)
                {
                    // 屏幕比图片细长，图片上下与屏幕对齐，图片左右被裁剪

                    float heightRate = Screen.height / textureOriginSize.y;

                    int img_x = (int)(heightRate * textureOriginSize.x);

                    float offset_x = (img_x - Screen.width) / 2f / img_x;
                    float w = (float)Screen.width / img_x;

                    img.uvRect = new Rect(offset_x, 0, w, 1);

                    //Debug.Log($"heightRate={heightRate} img_x={img_x}");
                }
                else
                {
                    // 屏幕比图片更方，图片左右与屏幕对齐，图片上下被裁剪

                    float widthRate = (float)Screen.width / textureOriginSize.x;

                    int img_y = (int)(widthRate * textureOriginSize.y);

                    float offset_y = (img_y - Screen.height) / 2f / img_y;
                    float h = (float)Screen.height / img_y;

                    img.uvRect = new Rect(0, offset_y, 1, h);

                    //Debug.Log($"widthRate={widthRate} img_y={img_y}");
                }
            }
            else
            {
                // 横屏

                if (texturexyRate > screenxyRate)
                {
                    // 图片比屏幕细长，图片上下与屏幕对齐，图片左右被裁剪

                    float heightRate = Screen.height / textureOriginSize.y;

                    int img_x = (int)(heightRate * textureOriginSize.x);

                    float offset_x = (img_x - Screen.width) / 2f / img_x;
                    float w = (float)Screen.width / img_x;

                    img.uvRect = new Rect(offset_x, 0, w, 1);

                    //Debug.Log($"heightRate={heightRate} img_x={img_x}");
                }
                else
                {
                    // 图片比屏幕更方，图片左右与屏幕对齐，图片上下被裁剪

                    float widthRate = (float)Screen.width / textureOriginSize.x;

                    int img_y = (int)(widthRate * textureOriginSize.y);

                    float offset_y = (img_y - Screen.height) / 2f / img_y;
                    float h = (float)Screen.height / img_y;

                    img.uvRect = new Rect(0, offset_y, 1, h);

                    //Debug.Log($"widthRate={widthRate} img_y={img_y}");
                }
            }

            rt.ForceUpdateRectTransforms();
        }

        public void AddRedPoint(string id, UGuiRedPoint p)
        {
            MapRedPoint ??= new();

            MapRedPoint[id] = p;
        }

        public void RemoveRedPoint(string id)
        {
            if (MapRedPoint != null)
            {
                MapRedPoint.Remove(id);
            }
        }

        public void HandleEventRedPoint(DEF.Event ev)
        {
            if (ev is DEF.Client.EvRedPoint ev_red_point)
            {
                if (MapRedPoint != null)
                {
                    MapRedPoint.TryGetValue(ev_red_point.RedPointId, out var p);

                    if (p != null)
                    {
                        p.gameObject.SetActive(ev_red_point.IsOn);
                    }
                }
            }
        }
    }

    public abstract class ViewFactory
    {
        public abstract string GetName();

        public virtual bool IsMultiInstance()
        {
            return false;
        }

        public virtual bool DisableOpenAndCloseAudio()
        {
            return true;
        }

        public abstract string GetComponentName();

        public abstract string GetParentName();

        public abstract View CreateView();
    }

    public abstract class ViewFactory<T> : ViewFactory where T : View, new()
    {
        public override string GetName()
        {
            return typeof(T).Name;
        }

        public override string GetComponentName()
        {
            return GetName().Replace("View", "");
        }

        public override View CreateView()
        {
            return new T();
        }
    }
}

#endif