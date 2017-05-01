using System;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Messages;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class FooterFragment : Fragment
    {

        private View _errorView;
        private TextView _lastUpdateTextView;
        private View _rootView;
        private ImageView _lastUpdateIcon;

        public DateTime LastUpdate
        {
            set
            {
                if (_lastUpdateTextView == null) return;

                _lastUpdateTextView.Text = value.AsString();
            }
        }

        private bool ShowConnectivityWarning
        {
            set
            {
                if (_rootView == null) return;

                _errorView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                _rootView.SetBackgroundColor(new Color(ContextCompat.GetColor(Application.Context, value ? Resource.Color.warning_color : Resource.Color.colorBackground)));
                _lastUpdateTextView.SetTextColor(value ? new Color(255, 255, 255) : new Color(ContextCompat.GetColor(Application.Context, Resource.Color.secondary_text_default_material_light)));
                _lastUpdateIcon.SetImageDrawable(ContextCompat.GetDrawable(Application.Context, value ? Resource.Drawable.ic_update_white : Resource.Drawable.ic_update));
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_footer, container, false);

            _errorView = view.FindViewById(Resource.Id.view_offline_info);
            _rootView = view.FindViewById(Resource.Id.view_root);
            _lastUpdateTextView = view.FindViewById<TextView>(Resource.Id.text_info);
            _lastUpdateIcon = view.FindViewById<ImageView>(Resource.Id.icon_update);

            ShowConnectivityWarning = !ConnectivityStatus.IsConnected;
            LastUpdate = DateTime.MinValue;

            Messaging.Status.Network.Subscribe(this, b => ShowConnectivityWarning = !b);

            return view;
        }
    }
}