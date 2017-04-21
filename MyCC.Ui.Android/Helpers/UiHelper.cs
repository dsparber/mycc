using Android.Content;
using Android.Support.V4.App;
using Android.Util;

namespace MyCC.Ui.Android.Helpers
{
    public static class UiHelper
    {
        public static float DpToPx(this Context context, float valueInDp)
        {
            var metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }


        public static void SetFragmentVisibility(this FragmentManager fm, Fragment fragment, bool visible)
        {

            var ft = fm.BeginTransaction();

            if (visible && fragment.IsHidden)
            {
                ft.Show(fragment);
                ft.Commit();
            }
            if (!visible && !fragment.IsHidden)
            {
                ft.Hide(fragment);
                ft.Commit();
            }
        }


    }
}