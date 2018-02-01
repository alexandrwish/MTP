using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;

namespace MTP.Droid
{
    [Activity(Label = "Details", ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = ".MainActivity")]
    public class BrowseItemDetailActivity : BaseActivity
    {
        public const int RequestCode = 12;

        protected override int LayoutResource => Resource.Layout.activity_item_details;

        private FloatingActionButton _deleteButton;
        private ItemDetailViewModel _viewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var data = Intent.GetStringExtra("data");

            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(data);
            _viewModel = new ItemDetailViewModel(item);
            _deleteButton = FindViewById<FloatingActionButton>(Resource.Id.delete_button);

            FindViewById<TextView>(Resource.Id.description).Text = item.Description;

            SupportActionBar.Title = item.Text;
            _deleteButton.Click += DeleteButton_Click;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            _viewModel.DeleteItemCommand.Execute(null);
            BrowseFragment.ViewModel.LoadItemsCommand.Execute(null);
            Finish();
        }
    }
}