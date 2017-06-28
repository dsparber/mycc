using System;
using System.Net;
using MyCC.Core.Helpers;
using MyCC.Forms.iOS.Helpers;
using MyCC.Forms.Resources;
using MyCC.Ui.Helpers;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ErrorDialog))]
namespace MyCC.Forms.iOS.Helpers
{
    public class ErrorDialog : IErrorDialog
    {

        public void Display(Exception e)
        {
            string message;

            if (e is WebException)
            {
                message = I18N.NetworkError;
            }
            else
            {
                message = I18N.GeneralError;
            }
            e.LogError();

            Display(message);
        }

        public void Display(string errorText)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var alert = UIAlertController.Create(I18N.Error, errorText, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(I18N.Ok, UIAlertActionStyle.Default, action => { }));
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
            });
        }
    }
}
