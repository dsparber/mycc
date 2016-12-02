using System;
using System.Collections.Generic;
using MyCryptos.Forms.Resources;
using Xamarin.Forms;

// TODO Refactor
namespace MyCryptos.Forms.Messages
{
    public class ErrorMessageHandler
    {
        public static readonly ErrorMessageHandler Instance = new ErrorMessageHandler();

        private bool networkAlertDisplayed;

        private ErrorMessageHandler()
        {
            //Messaging.NetworkError.Subscribe(this, DisplayNetworkAlert);
            //Messaging.UpdatingExchangeRates.Subscribe(this, new List<Tuple<MessageInfo, Action>> { Tuple.Create(MessageInfo.Started, () => networkAlertDisplayed = false) });
        }

        private void DisplayNetworkAlert()
        {
            if (networkAlertDisplayed) return;

            networkAlertDisplayed = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert(I18N.Error, I18N.NetworkError, I18N.Ok);
            });
        }
    }
}
