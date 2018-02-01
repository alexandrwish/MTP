using System;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.App;
using Android.Content;

namespace MTP.Droid
{
    public class BrowseFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public static BrowseFragment NewInstance() => new BrowseFragment {Arguments = new Bundle()};

        private BrowseItemsAdapter _adapter;
        private SwipeRefreshLayout _refresher;

        private ProgressBar _progress;
        public static ItemsViewModel ViewModel { get; private set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = new ItemsViewModel();

            var view = inflater.Inflate(Resource.Layout.fragment_browse, container, false);
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(_adapter = new BrowseItemsAdapter(Activity, ViewModel));

            _refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.SetColorSchemeColors(Resource.Color.accent);

            _progress = view.FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            _progress.Visibility = ViewStates.Gone;

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            _refresher.Refresh += Refresher_Refresh;
            _adapter.ItemClick += Adapter_ItemClick;
            _adapter.ItemLongClick += Adapter_ItemLongClick;

            if (ViewModel.Items.Count == 0)
                ViewModel.LoadItemsCommand.Execute(null);
        }

        public override void OnStop()
        {
            base.OnStop();
            _refresher.Refresh -= Refresher_Refresh;
            _adapter.ItemClick -= Adapter_ItemClick;
        }

        private void Adapter_ItemClick(object sender, RecyclerClickEventArgs e)
        {
            var item = ViewModel.Items[e.Position];
            var intent = new Intent(Activity, typeof(BrowseItemDetailActivity));

            intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
            StartActivity(intent);
        }

        private void Adapter_ItemLongClick(object sender, RecyclerClickEventArgs e)
        {
            var item = ViewModel.Items[e.Position];
            var intent = new Intent(Activity, typeof(AddItemActivity));

            intent.PutExtra("data", Newtonsoft.Json.JsonConvert.SerializeObject(item));
            StartActivity(intent);
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            ViewModel.LoadItemsCommand.Execute(null);
            _refresher.Refreshing = false;
        }

        public void BecameVisible()
        {
        }
    }

    internal class BrowseItemsAdapter : BaseRecycleViewAdapter
    {
        private readonly ItemsViewModel _viewModel;

        public BrowseItemsAdapter(Activity activity, ItemsViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.Items.CollectionChanged += (sender, args) => { activity.RunOnUiThread(NotifyDataSetChanged); };
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            const int id = Resource.Layout.item_browse;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);

            var vh = new MyViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _viewModel.Items[position];

            // Replace the contents of the view with that element
            if (!(holder is MyViewHolder myHolder)) return;
            myHolder.TextView.Text = item.Text;
            myHolder.DetailTextView.Text = item.Description;
        }

        public override int ItemCount => _viewModel.Items.Count;
    }

    public class MyViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; }
        public TextView DetailTextView { get; }

        public MyViewHolder(View itemView, Action<RecyclerClickEventArgs> clickListener,
            Action<RecyclerClickEventArgs> longClickListener) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Android.Resource.Id.Text1);
            DetailTextView = itemView.FindViewById<TextView>(Android.Resource.Id.Text2);
            itemView.Click += (sender, e) =>
                clickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
            itemView.LongClick += (sender, e) =>
                longClickListener(new RecyclerClickEventArgs {View = itemView, Position = AdapterPosition});
        }
    }
}