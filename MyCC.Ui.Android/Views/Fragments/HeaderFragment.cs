using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class HeaderFragment : Fragment
    {
        private TextView _mainTextView;
        private TextView _infoTextView;

        public string MainText
        {
            get { return _mainTextView.Text; }
            set { _mainTextView.Text = value; }
        }

        public string InfoText
        {
            get { return _infoTextView.Text; }
            set { _infoTextView.Text = value; }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_header, container, false);

            _mainTextView = view.FindViewById<TextView>(Resource.Id.main_text);
            _infoTextView = view.FindViewById<TextView>(Resource.Id.info_text);

            MainText = "MyCC";
            InfoText = "My Cryptocurrencies";

            return view;
        }
    }
}