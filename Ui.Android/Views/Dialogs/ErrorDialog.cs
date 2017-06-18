using System;
using System.Net;
using MyCC.Core.Helpers;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Dialogs;
using MyCC.Ui.Helpers;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(ErrorDialog))]
namespace MyCC.Ui.Android.Views.Dialogs
{
    public class ErrorDialog : IErrorDialog
    {

        public void Display(Exception e)
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

        public void Display(string errorText)
        {
            MyccActivity.CurrentInstance.RunOnUiThread(() =>
            {
                MyccActivity.CurrentInstance.ShowInfoDialog(Application.Context.GetString(Resource.String.Error), errorText);
            });
        }
    }
}
