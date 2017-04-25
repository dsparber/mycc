using System;
using System.Net;
using Android.App;
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

            MyccActivity.CurrentInstance.RunOnUiThread(() =>
            {
                context.ShowInfoDialog(Resource.String.Error, messageId);
            });
        }
    }
}
