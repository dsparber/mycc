using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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
            Debug.WriteLine(e);
            HockeyApp.MetricsManager.TrackEvent($"{e.GetType().Name}: {e.Message}",
                new Dictionary<string, string> { { "error", e.ToString() } },
                new Dictionary<string, double> { { "time", DateTime.Now.Ticks } });

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
