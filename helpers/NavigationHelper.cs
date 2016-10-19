using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCryptos.helpers
{
    public static class NavigationHelper
    {
        public static Task PushOrPushModal(this INavigation navigation, Page page) {
            if (Modal)
            {
                return navigation.PushModalAsync(new NavigationPage(page));
            }else
            {
                return navigation.PushAsync(page);
            }
        }

        public static Task PopOrPopModal(this INavigation navigation)
        {
            if (Modal)
            {
                return navigation.PopModalAsync();
            }
            else
            {
                return navigation.PopAsync();
            }
        }

        public static bool Modal {
            get { return Device.OS != TargetPlatform.Android; }
        }
    }
}
