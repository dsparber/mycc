using Android.App;
using Android.Util;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Math = System.Math;

namespace MyCC.Ui.Android.Helpers
{
    public static class UiHelper
    {
        public static int DpToPx(this int value)
        {
            var metrics = Application.Context.Resources.DisplayMetrics;
            return (int)Math.Round(TypedValue.ApplyDimension(ComplexUnitType.Dip, value, metrics), 0);
        }

        public static void SetFragmentVisibility(this FragmentManager fm, Fragment fragment, bool visible)
        {
            try
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
            catch (RuntimeException)
            {
                /* Thrown when this methode is called after onSaveInstanceState */
            }
        }


    }
}