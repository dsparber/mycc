using System;
using System.Net;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;

namespace MyCryptos.Forms.view.overlays
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
                throw e;
            }

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
