using System;
using System.Net;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;

namespace MyCC.Ui.Android.Views.Dialogs
{
    public static class ErrorDialog
    {

        public static void Display(Exception e)
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

            MyccActivity.CurrentInstance.RunOnUiThread(() =>
            {
                MyccActivity.CurrentInstance.ShowInfoDialog(Resource.String.Error, messageId);
            });
        }
    }
}
