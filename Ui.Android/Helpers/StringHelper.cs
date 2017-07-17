using System;
using Android.App;
using MyCC.Core.Helpers;

namespace MyCC.Ui.Android.Helpers
{
    public static class StringHelper
    {
        public static string AsString(this DateTime dateTime)
        {
            return dateTime.AsString(Application.Context.GetString(Resource.String.Never));
        }

        public static string GetPlural(this int count, int noItems, int oneItem, int manyItems)
        {
            switch (count)
            {
                case 0: return Application.Context.Resources.GetString(noItems);
                case 1: return Application.Context.Resources.GetString(oneItem);
                default: return $"{count} {Application.Context.Resources.GetString(manyItems)}";
            }
        }
    }
}

