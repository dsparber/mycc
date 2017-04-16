using System;
using System.Net;
using Android.App;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Helpers;

namespace MyCC.Ui.Android.Views.Dialogs
{
    public static class ErrorDialog
    {

        public static void Display(Exception e)
        {
            int messageId;
            var context = Application.Context;

            if (e is WebException)
            {
                messageId = Resource.String.NetworkError;
            }
            else
            {
                messageId = Resource.String.GeneralError;
            }
            e.LogError();

            ((Activity)context).RunOnUiThread(() =>
            {
                context.ShowInfoDialog(Resource.String.Error, messageId);
            });
        }
    }
}
