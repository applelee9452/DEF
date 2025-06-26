#if DEF_CLIENT

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UGuiRawImageAni : MonoBehaviour
{
    public int XCount = 1;
    public int YCount = 1;
    public float TimeSpan = 0.1f;

    private float OffsetX;
    private float OffsetY;
    private RawImage RawImage;

    void Start()
    {
        RawImage = GetComponent<RawImage>();

        OffsetX = 1f / XCount;
        OffsetY = 1f / YCount;

        StartCoroutine(Ani());
    }

    private void OnEnable()
    {
        StartCoroutine(Ani());
    }

    private void OnDisable()
    {
        StopCoroutine(Ani());
    }

    private IEnumerator Ani()
    {
        if (RawImage == null) yield return 0;

        int x = 0;
        int y = 0;

        while (true)
        {
            if (y >= YCount)
            {
                y = 0;
            }

            x = 0;

            while (x < XCount)
            {
                RawImage.uvRect = new Rect(x * OffsetX, y * OffsetY, OffsetX, OffsetY);

                yield return new WaitForSeconds(TimeSpan);

                x++;
            }

            y++;
        }
    }
}

#endif