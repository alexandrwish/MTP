using System;
using Android.Content;
using Android.Widget;
using MTP.Droid;
using Plugin.CurrentActivity;

namespace MTP.ViewModel
{
    public static class LoginViewModelExtension
    {
        public static void ExtendLogin(bool success, Action callback)
        {
            var pref = MainApplication.Current.preference;
            var currentActivity = CrossCurrentActivity.Current.Activity;
            if (success)
            {
                pref.Edit().PutBoolean(MainApplication.USER_LOGIN, true).Apply();
                currentActivity.StartActivity(new Intent(currentActivity, typeof(SplashActivity)));
                currentActivity.Finish();
            }
            else
            {
                var count = pref.GetInt(MainApplication.LOGIN_COUNT, 0) + 1;
                if (count > 5)
                {
                    pref.Edit().PutInt(MainApplication.LOGIN_COUNT, 0).Apply();
                    callback();
                    currentActivity.Finish();
                }
                else
                {
                    pref.Edit().PutInt(MainApplication.LOGIN_COUNT, count).Apply();
                    Toast.MakeText(currentActivity, "Wrong login info", ToastLength.Short).Show();
                }
            }
        }
    }
}