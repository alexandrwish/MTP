using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;

namespace MTP.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_login;
        private TextInputEditText Login { get; set; }
        private TextInputEditText Password { get; set; }
        private LoginViewModel ViewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ViewModel = new LoginViewModel();
            Login = FindViewById<TextInputEditText>(Resource.Id.input_login);
            Password = FindViewById<TextInputEditText>(Resource.Id.input_password);
            FindViewById(Resource.Id.login_btn).Click += delegate
            {
                var record = new LoginRecord
                {
                    Login = Login.Text,
                    Password = Password.Text
                };
                ViewModel.LoginCommand.Execute(record);
            };
            FindViewById(Resource.Id.link_signup).Click += delegate
            {
                Toast.MakeText(this, "здесь могла быть ваша реклама", ToastLength.Long).Show();
            };
        }
    }
}