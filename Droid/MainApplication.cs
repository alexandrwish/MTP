using System;
using System.Runtime.InteropServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Preferences;

using Plugin.CurrentActivity;

namespace MTP.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        private static string CERT_NAME = "certificate.name";
        public static string USER_LOGIN = "user.login";
        public static string LOGIN_COUNT = "login.count";

        public static MainApplication Current { get; set; }

        public ISharedPreferences preference { get; set; }

    public MainApplication(IntPtr handle, JniHandleOwnership transer)
        : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            App.Initialize();
            Current = this;
            preference = PreferenceManager.GetDefaultSharedPreferences(this);
        }
        
        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {

        }

        public void OnActivityPaused(Activity activity)
        {

        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {

        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {

        }

        public String getCertificateAlias()
        {
            return preference.GetString(CERT_NAME, null);
        }

        public void SaveCertificateAlias(string alias) {
            preference.Edit().PutString(CERT_NAME, alias).Apply();    
        }
        
        public void RemoveCertificateAlias() {
            preference.Edit().PutString(CERT_NAME, null).Apply();    
        }
    }
}