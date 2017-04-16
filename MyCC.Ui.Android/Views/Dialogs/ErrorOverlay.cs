using System;
using System.Net;
using Android.Content;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Helpers;
using Xamarin.Forms;

namespace MyCC.Ui.Android.Views.Dialogs
{
    public static class ErrorOverlay
    {

        public static void Display(Exception e, Context context)
        {
            int messageId;

            if (e is WebException)
            {
                messageId = Resource.String.NetworkError;
            }
            else
            {
                messageId = Resource.String.GeneralError;
            }
            e.LogError();

            Device.BeginInvokeOnMainThread(() =>
            {
                context.ShowInfoDialog(Resource.String.Error, messageId);
            });
        }
    }
}
