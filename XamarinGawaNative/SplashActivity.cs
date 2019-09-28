using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace XamarinGawaNative
{
 [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

        }

        protected override void OnResume()
        {
            base.OnResume();
            Task delay = new Task(() => { DelayedStartup(); });
            delay.Start();
        }

        void DelayedStartup()
        {
            Log.Debug(TAG, "Loaded delay for screen");
            //await Task.Delay(8000); // Simulate a bit of startup work.
            Log.Debug(TAG, "Run Main");
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}