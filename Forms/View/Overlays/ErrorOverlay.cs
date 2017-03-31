using System;
using System.Net;
using MyCC.Core.Helpers;
using MyCC.Forms.Resources;
using Xamarin.Forms;

namespace MyCC.Forms.View.Overlays
{
    public static class ErrorOverlay
    {

        public static void Display(Exception e)
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

            Device.BeginInvokeOnMainThread(() =>
            {
                if (e is WebException)
                {
                    Application.Current.MainPage.DisplayAlert(I18N.Error, message, I18N.Ok);
                }
            });
        }
    }
}
