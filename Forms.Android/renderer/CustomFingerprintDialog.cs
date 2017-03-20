using Android.OS;
using Android.Views;
using Android.Widget;
using Plugin.Fingerprint.Dialog;

namespace MyCC.Forms.Android.renderer
{
    public class CustomFingerprintDialog : FingerprintDialogFragment
    {
        public override global::Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            view.FindViewById<Button>(Resource.Id.fingerprint_btnCancel).Visibility = ViewStates.Invisible;
            view.FindViewById<ImageView>(Resource.Id.fingerprint_imgFingerprint).SetImageDrawable(Xamarin.Forms.Forms.Context.GetDrawable(Resource.Drawable.Fingerprint));
            view.FindViewById<ImageView>(Resource.Id.fingerprint_imgFingerprint).SetPadding(0, 50, 0, 0);
            return view;
        }

    }
}