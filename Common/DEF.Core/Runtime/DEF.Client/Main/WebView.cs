#if DEF_CLIENT

using UnityEngine;

namespace DEF.Client
{
    public class WebView
    {
        GameObject Go { get; set; }
        //UniWebView UniWebView { get; set; }

        public WebView()
        {
            //var go = GameObject.Find("UniWebView");
            //UniWebView = go.GetComponent<UniWebView>();

            //UniWebView.OnPageStarted += (UniWebView webView, string url1) =>
            //{
            //};
            //UniWebView.OnPageFinished += (UniWebView webView, int statusCode, string url1) =>
            //{
            //};
            //UniWebView.OnPageErrorReceived += (UniWebView webView, int errorCode, string errorMessage) =>
            //{
            //};
            //UniWebView.OnMessageReceived += (UniWebView webView, UniWebViewMessage message) =>
            //{
            //};

            //UniWebView.Frame = new Rect(0, 0, Screen.width, Screen.height);
            //UniWebView.SetBackButtonEnabled(true);
            //UniWebView.SetShowToolbar(true, true, false);
        }

        public void Show(string url)
        {
            //UniWebView.Load(url);
            //UniWebView.Show();
        }

        public void Hide()
        {
            //UniWebView.Stop();
            //UniWebView.Hide();
        }

        public void Close()
        {
            //if (Go != null)
            //{
            //    Object.DestroyImmediate(Go);
            //    Go = null;
            //    UniWebView = null;
            //}
        }
    }
}

#endif
