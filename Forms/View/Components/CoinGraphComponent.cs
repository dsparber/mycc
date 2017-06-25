using System;
using MyCC.Core.Account.Storage;
using MyCC.Core.Settings;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Pages;
using MyCC.Ui;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components
{
    public class CoinGraphComponent : ContentView
    {
        private readonly HybridWebView _webView;

        public CoinGraphComponent(INavigation navigation)
        {
            _webView = new HybridWebView("Html/pieChart.html") { LoadFinished = UpdateView };

            _webView.RegisterCallback("selectedCallback", id =>
            {
                var element = AccountStorage.Instance.AllElements.Find(e => e.Id == Convert.ToInt32(id));
                if (element != null) Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new AccountView(element)));
            });

            Content = _webView;
            HeightRequest = 500;

            UpdateView();

            Messaging.Update.Rates.Subscribe(this, UpdateView);
            Messaging.Update.Balances.Subscribe(this, UpdateView);
            Messaging.Status.CarouselPosition.Subscribe(this, UpdateView);
        }

        private void UpdateView()
        {
            Device.BeginInvokeOnMainThread(() => _webView.CallJsFunction(UiUtils.Get.Assets.GrapItemsJsFor(ApplicationSettings.StartupCurrencyAssets)));
        }
    }
}
