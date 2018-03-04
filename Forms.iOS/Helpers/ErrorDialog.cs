using System;
using System.Net;
using MyCC.Core.Helpers;
using MyCC.Forms.iOS.Helpers;
using MyCC.Forms.Resources;
using MyCC.Ui.Helpers;
using UIKit;
using System.Linq;
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
                // message = I18N.GeneralError;
                throw e;
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

                var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                var navigationController = rootViewController as UINavigationController;
                if (navigationController != null)
                {
                    rootViewController = navigationController.ViewControllers.First();
                }
                var tabBarController = rootViewController as UITabBarController;
                if (tabBarController != null)
                {
                    rootViewController = tabBarController.SelectedViewController;
                }
                rootViewController.PresentViewController(alert, true, null);
            });
        }
    }
}
