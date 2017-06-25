using System;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace MyCC.Ui.Android.Views.Dialogs
{
    public class FingerprintDialog : DialogFragment
    {
        private ImageView _fingerprintIcon;
        private TextView _statusText;

        public new static Action OnCancel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            OnCreate(savedInstanceState);

            Dialog.SetTitle(Resource.String.UnlockApplication);

            var view = inflater.Inflate(Resource.Layout.fingerprint_dialog_container, container, false);

            view.FindViewById<Button>(Resource.Id.cancel_button).Click += (sender, args) => { Dismiss(); OnCancel?.Invoke(); };
            view.FindViewById<Button>(Resource.Id.cancel_button).SetText(Resource.String.Cancel);

            _fingerprintIcon = view.FindViewById<ImageView>(Resource.Id.fingerprint_icon);
            _statusText = view.FindViewById<TextView>(Resource.Id.fingerprint_status);

            return view;
        }

        public void AlertError(Activity activity)
        {
            _fingerprintIcon.StartAnimation(AnimationUtils.LoadAnimation(activity, Resource.Animation.shake));
            _statusText.SetText(Resource.String.AuthenticationFailed);
            _statusText.SetTextColor(new Color(ContextCompat.GetColor(Application.Context, Resource.Color.warning_color)));
            Task.Run(() => Task.Delay(3000).ContinueWith(t => activity.RunOnUiThread(() =>
            {
                _statusText.SetText(Resource.String.fingerprint_hint);
                _statusText.SetTextColor(new Color(ContextCompat.GetColor(Application.Context, Resource.Color.hint_color)));
            })));
        }
    }
}