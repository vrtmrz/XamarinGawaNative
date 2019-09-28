using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace XamarinGawaNative
{
    class HybridWebChromeViewClient : WebChromeClient
    {
        public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            base.OnGeolocationPermissionsShowPrompt(origin, callback);
        }
        /// <summary>
        /// Confirmを表示する
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
        {
            var dialog = new AlertDialog.Builder(view.Context);
            var dlg = dialog.SetTitle("?")
                .SetIcon(ContextCompat.GetDrawable(view.Context, Android.Resource.Drawable.IcDialogAlert))
                .SetMessage(message)
                .SetNegativeButton(Android.Resource.String.No, (object sender, DialogClickEventArgs args) =>
                {
                    result.Cancel();
                }).SetPositiveButton(Android.Resource.String.Yes, (object sender, DialogClickEventArgs args) =>
                {
                    result.Confirm();
                }).
                Create();
            dlg.SetCanceledOnTouchOutside(false);

            dlg.Show();
            return true;
        }
        /// <summary>
        /// Alertを表示する
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            var dialog = new AlertDialog.Builder(view.Context);
            var dlg = dialog.SetTitle("!")
                .SetIcon(ContextCompat.GetDrawable(view.Context, Android.Resource.Drawable.IcDialogAlert))
                .SetMessage(message)
                .SetPositiveButton(Android.Resource.String.Ok, (object sender, DialogClickEventArgs args) =>
                {
                    result.Confirm();
                }).Create();
            dlg.SetCanceledOnTouchOutside(false);
            dlg.Show();
            return true;
        }

    }
}