using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class SettingsContainerFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_settings_container, container, false);
        }
    }
}