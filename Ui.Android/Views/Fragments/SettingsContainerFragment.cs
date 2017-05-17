using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using MyCC.Ui.Android.Helpers;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class SettingsContainerFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_settings_container, container, false);

            ChildFragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, new SettingsFragment()).Commit();

            var header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            header.InfoText = Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0).VersionName;

            var activityRootView = view.FindViewById(Resource.Id.fragment_root);
            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) =>
            {
                if (IsDetached || !IsAdded) return;
                ChildFragmentManager.SetFragmentVisibility(header, activityRootView.Height > 400.DpToPx());
            };

            return view;
        }
    }
}