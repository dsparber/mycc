using System;
using MyCryptos.Core.Constants;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;

namespace MyCryptos.message
{
    public class ErrorMessageHandler
    {
        public static ErrorMessageHandler Instance = new ErrorMessageHandler();

        bool networkAlertDisplayed;

        public ErrorMessageHandler()
        {
            MessagingCenter.Subscribe<Exception>(this, MessageConstants.NetworkError, e => displayNetworkAlert());
            MessagingCenter.Subscribe<string>(this, MessageConstants.StartedFetching, str => networkAlertDisplayed = false);
        }

        void displayNetworkAlert()
        {
            if (!networkAlertDisplayed)
            {
                networkAlertDisplayed = true;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert(I18N.Error, I18N.NetworkError, I18N.Ok);
                });
            }
        }
    }
}
