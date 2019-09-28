using System;
using System.Collections.Concurrent;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Newtonsoft.Json;

namespace XamarinGawaNative
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ConfigurationChanges =Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize, LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : AppCompatActivity
    {
        WebView webView;
        bool initialized;
        private ConcurrentQueue<string[]> EventWaiting = new ConcurrentQueue<string[]>();
        private object lockObj = new object();
        private bool eventrunning = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            
            webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;

            //JavaScript側に露出するオブジェクト名
            webView.AddJavascriptInterface(new WebAppInterface(this), "CS");
            webView.Settings.DatabaseEnabled = true;
            webView.Settings.DomStorageEnabled = true;

            webView.Settings.SetGeolocationEnabled(true);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {  //JB以上は明示的に許可する必要がある
                //（file://でのscriptタグと、file://でのアクセス）
                webView.Settings.AllowUniversalAccessFromFileURLs = true;
                webView.Settings.AllowFileAccessFromFileURLs = true;

            }
            
#if DEBUG
            WebView.SetWebContentsDebuggingEnabled(true);
#endif
            webView.SetWebViewClient(new HybridWebViewClient(this));
            webView.SetWebChromeClient(new HybridWebChromeViewClient());
            
            if (savedInstanceState != null)
            {
                webView.RestoreState(savedInstanceState);
            }
            else
            {
                NavigateTop();
            }
            if (Intent != null)
            {
                OnNewIntent(Intent);
            }
        }
        protected override void OnStart()
        {
            base.OnStart();
            RaiseEventBrowser("onresume", "");
        }
        public override void OnSaveInstanceState(Bundle outState, PersistableBundle outPersistentState)
        {
            webView.SaveState(outState);
            base.OnSaveInstanceState(outState, outPersistentState);
        }
        protected override void OnPause()
        {
            RaiseEventBrowser("onpause", "");
            webView.PauseTimers();
            base.OnPause();
        }
        protected override void OnResume()
        {
            webView.ResumeTimers();
            RaiseEventBrowser("onresume", "");
            base.OnResume();
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            webView.SaveState(outState);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            //何か処理を入れるのであればここらへん
            if (intent.Action == Intent.ActionView && intent.Scheme == "something")
            {
                var uri = intent.Data;
                var code = uri.GetQueryParameter("foo");
                var state = uri.GetQueryParameter("bar");
                //DO SOMETHING
            }
            if (intent.Action == Intent.ActionView)
            {
                var ds = intent.DataString.ToString();
                //DO SOMETHING
            }
            if (intent.Action == Intent.ActionSend)
            {
                //DO SOMETHING
            }
        }
        /// <summary>
        /// WebViewが初期化されたら呼ばれる。
        /// （イベントの空撃ちを防ぐため）
        /// </summary>
        public void Initialized()
        {
            this.initialized = true;
            RaiseEventBrowser("", ""); //
        }
        
        /// <summary>
        /// WebViewから来たURIをStartActivityする。
        /// </summary>
        /// <param name="uristring">起動URI</param>
        public void Emit(string uristring)
        {
            var uri = Android.Net.Uri.Parse(uristring);
            var i = new Intent(Intent.ActionView, uri);
            StartActivity(i);
        }


        /// <summary>
        /// WebViewにイベントを発生させる
        /// </summary>
        /// <param name="eventName">イベント名</param>
        /// <param name="eventArgs">イベント引数</param>
        public void RaiseEventBrowser(string eventName, object eventArgs)
        {
            lock (lockObj)
            {
                var eventArgument = JsonConvert.SerializeObject(eventArgs);
                if (eventName != "")
                    EventWaiting.Enqueue(new string[] { eventName, eventArgument });
                if (!initialized) return;
                if (eventrunning) return;
                eventrunning = true;
                try
                {
                    if (EventWaiting.Any())
                    {
                        var item = new string[] { };
                        while (EventWaiting.TryDequeue(out item))
                        {
                            var itemx = item;
                            RunOnUiThread(() =>
                            {
                                webView.EvaluateJavascript($"window.dispatchEvent(new CustomEvent('{itemx[0]}', {{ detail: {itemx[1]} }}));", null);
                            });
                        }
                    }
                }
                finally
                {
                    eventrunning = false;
                }
            }
        }
        private void NavigateTop()
        {
            RunOnUiThread(() =>
            {
#if DEBUG
                //debug中はローカルから配信したい場合は以下を活かす
                //webView.LoadUrl("http://10.0.2.2:8080/#");
                webView.LoadUrl("file:///android_asset/index.html#");
#else 
                webView.LoadUrl("file:///android_asset/index.html#");
#endif
            });
        }

    }
}

