using Android.OS;
using Android.Views;
using Android.Widget;

namespace MTP.Droid
{
    public class AboutFragment : Android.Support.V4.App.Fragment, IFragmentVisible
    {
        public static AboutFragment NewInstance() =>
            new AboutFragment {Arguments = new Bundle()};

        private AboutViewModel ViewModel { get; set; }

        private Button _learnMoreButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_about, container, false);
            ViewModel = new AboutViewModel();
            _learnMoreButton = view.FindViewById<Button>(Resource.Id.button_learn_more);
            return view;
        }

        public override void OnStart()
        {
            base.OnStart();
            _learnMoreButton.Click += LearnMoreButton_Click;
        }

        public override void OnStop()
        {
            base.OnStop();
            _learnMoreButton.Click -= LearnMoreButton_Click;
        }

        public void BecameVisible()
        {
        }

        private void LearnMoreButton_Click(object sender, System.EventArgs e)
        {
            ViewModel.OpenWebCommand.Execute(null);
        }
    }
}