﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace MTP.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/SplashTheme", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var newIntent = MainApplication.Current.preference.GetBoolean(MainApplication.USER_LOGIN, true)
                ? new Intent(this, typeof(MainActivity))
                : new Intent(this, typeof(LoginActivity));
            newIntent.AddFlags(ActivityFlags.ClearTop);
            newIntent.AddFlags(ActivityFlags.SingleTop);

            StartActivity(newIntent);
            Finish();
        }
    }
}