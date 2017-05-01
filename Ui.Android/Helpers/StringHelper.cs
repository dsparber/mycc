using System;
using Android.App;

namespace MyCC.Ui.Android.Helpers
{
    public static class StringHelper
    {
        public static string AsString(this DateTime dateTime)
        {
            return Ui.Helpers.StringHelper.AsString(dateTime, Application.Context.GetString(Resource.String.Never));
        }
    }
}

