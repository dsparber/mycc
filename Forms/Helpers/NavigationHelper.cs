using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCC.Forms.helpers
{
    public static class NavigationHelper
    {
        public static Task PushOrPushModal(this INavigation navigation, Page page)
        {
            return Modal ? navigation.PushModalAsync(new NavigationPage(page) { BarTextColor = Color.White }) : navigation.PushAsync(page);
        }

        public static Task PopOrPopModal(this INavigation navigation)
        {
            return Modal ? navigation.PopModalAsync() : navigation.PopAsync();
        }

        private static bool Modal => Device.OS != TargetPlatform.Android;
    }
}
