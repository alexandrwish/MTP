using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Support.Design.Widget;

namespace MTP.Droid
{
    [Activity(Label = "AddItemActivity")]
    public class AddItemActivity : Activity
    {
        private FloatingActionButton _saveButton;
        private EditText _title, _description;
        private Item _item;

        private ItemsViewModel ViewModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ViewModel = BrowseFragment.ViewModel;

            // Create your application here
            SetContentView(Resource.Layout.activity_add_item);
            _saveButton = FindViewById<FloatingActionButton>(Resource.Id.save_button);
            _title = FindViewById<EditText>(Resource.Id.txtTitle);
            _description = FindViewById<EditText>(Resource.Id.txtDesc);

            var data = Intent.GetStringExtra("data");
            if (data == null)
            {
                _item = new Item();
            }
            else
            {
                _item = Newtonsoft.Json.JsonConvert.DeserializeObject<Item>(data);
                _title.Text = _item.Text;
                _description.Text = _item.Description;
            }

            _saveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            _item.Text = _title.Text;
            _item.Description = _description.Text;
            if (_item.Id == null)
            {
                _item.Id = Guid.NewGuid().ToString();
                ViewModel.AddItemCommand.Execute(_item);
            }
            else
            {
                ViewModel.UpdateItemComand.Execute(_item);
            }

            Finish();
        }
    }
}