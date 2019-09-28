using System.Threading.Tasks;
using Android.Webkit;
using Android.Widget;
using Java.Interop;

namespace XamarinGawaNative
{
    /// <summary>
    /// WebAppに露出するオブジェクト
    /// 参照設定に、Mono.Android.Exportが必要
    /// このオブジェクトは結構脆弱。WebViewで任意のJSは呼ばないように。
    /// </summary>
    public class WebAppInterface : Java.Lang.Object
    {
        MainActivity main;
        public WebAppInterface(MainActivity activity) : base()
        {
            main = activity;
        }
        /// <summary>
        /// Toastを表示する
        /// </summary>
        /// <param name="toast"></param>
        [Export]
        [JavascriptInterface]
        public void ShowToast(string toast)
        {
            Toast.MakeText(main, toast, ToastLength.Short).Show();
        }
        /// <summary>
        /// JSにそのまま値を返す関数
        /// </summary>
        /// <param name="foo"></param>
        /// <returns></returns>
        [Export]
        [JavascriptInterface]
        public string HelloWorld(string foo)
        {
            return $"Hello World ,{foo}";
        }
        /// <summary>
        /// MainActivity経由でWebViewにイベントを起こす関数
        /// </summary>
        /// <param name="foo"></param>
        [Export]
        [JavascriptInterface]
        public void HelloWorldEvent(string foo)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                main.RaiseEventBrowser("helloworld", $"Hello World ,{foo}");
            });
        }
        /// <summary>
        /// 初期化された際にWebViewから呼んで、MainActivityに初期化完了を伝える
        /// </summary>
        [Export]
        [JavascriptInterface]
        public void Initialized()
        {
            main.Initialized();
        }
        /// <summary>
        /// WebViewから任意のURIを蹴りたいときに使う
        /// IntentのURIでも可。
        /// </summary>
        /// <param name="uri"></param>
        [Export]
        [JavascriptInterface]
        public void Emit(string uri)
        {
            main.Emit(uri);
        }
    }
}

