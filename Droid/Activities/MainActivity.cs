using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;

namespace MTP.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity
    {
        protected override int LayoutResource => Resource.Layout.activity_main;

        private ViewPager _pager;
        private TabsAdapter _adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _adapter = new TabsAdapter(this, SupportFragmentManager);
            _pager = FindViewById<ViewPager>(Resource.Id.viewpager);
            var tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            _pager.Adapter = _adapter;
            tabs.SetupWithViewPager(_pager);
            _pager.OffscreenPageLimit = 3;

            _pager.PageSelected += (sender, args) =>
            {
                var fragment = _adapter.InstantiateItem(_pager, args.Position) as IFragmentVisible;

                fragment?.BecameVisible();
            };

            Toolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.menu_edit:
                    {
                        var intent = new Intent(this, typeof(AddItemActivity));
                        StartActivity(intent);
                        break;
                    }
                    case Resource.Id.menu_logout:
                    {
                        MainApplication.Current.preference.Edit().PutBoolean(MainApplication.USER_LOGIN, false).Apply();
                        var intent = new Intent(this, typeof(SplashActivity));
                        StartActivity(intent);
                        Finish();
                        break;
                    }
                }
            };

            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetHomeButtonEnabled(false);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }
    }

    internal class TabsAdapter : FragmentStatePagerAdapter
    {
        private readonly string[] _titles;

        public override int Count => _titles.Length;

        public TabsAdapter(Context context, Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
            _titles = context.Resources.GetTextArray(Resource.Array.sections);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position) =>
            new Java.Lang.String(_titles[position]);

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return BrowseFragment.NewInstance();
                case 1: return AboutFragment.NewInstance();
            }

            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag) => PositionNone;
    }
}