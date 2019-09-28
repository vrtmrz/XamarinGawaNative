using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace XamarinGawaNative
{
    public class HybridWebViewClient : WebViewClient
    {
        private MainActivity activity;

        public HybridWebViewClient() : base()
        {

        }

        public HybridWebViewClient(MainActivity activity) : base()
        {
            this.activity = activity;
        }

        public override void OnLoadResource(WebView view, string url)
        {
            base.OnLoadResource(view, url);
        }

        public override void OnReceivedHttpAuthRequest(WebView view, HttpAuthHandler handler, string host, string realm)
        {
            base.OnReceivedHttpAuthRequest(view, handler, host, realm);
        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView webView, string url)
        {
            if (HookUrlLoad(url))
            {
                return true;
            }
            return base.ShouldOverrideUrlLoading(webView, url);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            if (HookUrlLoad(request.Url.ToString()))
            {
                return true;
            }
            return base.ShouldOverrideUrlLoading(view, request);

        }
        
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            var rep = InterceptByLocalProcess(request.Url.ToString());
            if (rep != null)
            {
                return rep;
            }
            return base.ShouldInterceptRequest(view, request);
        }

        [Obsolete]
        public override WebResourceResponse ShouldInterceptRequest(WebView view, string url)
        {

            var rep = InterceptByLocalProcess(url);
            if (rep != null)
            {
                return rep;
            }
            return base.ShouldInterceptRequest(view, url);

        }

        private bool HookUrlLoad(string url)
        {
            //URLのLoadをインターセプトするならここ。
            var scheme = "foo:";

            if (!url.StartsWith(scheme))
                return false;
            if (url.StartsWith(scheme + "//foo"))
            {
                //DO SOMETHING
                return true;
            }
            if (url.StartsWith(scheme + "//"))
                return false;

            var resources = url.Substring(scheme.Length).Split('?');
            var method = resources[0];
            //var parameters = resources.Length == 1 ? null : System.Web.HttpUtility.ParseQueryString(resources[1]);
            switch (method)
            {
                case "some":
                    //DO SOMETHING
                    break;
                case "thing":
                    //DO SOMETHING
                    break;
                case "hook":
                    //DO SOMETHING
                    break;
                default:
                    return true;
            }
            return true;
        }

        private WebResourceResponse InterceptByLocalProcess(string url)
        {
            //URLへのアクセスをインターセプトするならここで。
            return null;
        }

    }
}