#if DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UiHelper
{
    public static void MakeBgFitScreen(bool is_portrait, RectTransform rt, Vector2 textureOriginSize, RawImage img)
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
}

#endif