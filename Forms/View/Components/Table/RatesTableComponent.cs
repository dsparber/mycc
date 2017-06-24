using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Settings;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Pages;
using MyCC.Ui;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Forms.View.Components.Table
{
    public class RatesTableComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private readonly Dictionary<int, Action> _headerClickCallbacks;
        private static int _currentId;


        public RatesTableComponent(INavigation navigation)
        {
            _webView = new HybridWebView("Html/ratesTable.html") { LoadFinished = UpdateView };
            _headerClickCallbacks = new Dictionary<int, Action>();

            _webView.RegisterCallback("Callback", currencyId =>
            {
                Device.BeginInvokeOnMainThread(() => navigation.PushAsync(new CoinInfoView(currencyId, true)));
            });

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });

            _webView.RegisterCallback("HeaderClickedCallback", id =>
            {
                _headerClickCallbacks[int.Parse(id)].Invoke();
                UpdateView();
            });

            Content = _webView;

            Messaging.Update.Rates.Subscribe(this, UpdateView);
            Messaging.Update.Balances.Subscribe(this, UpdateView);
        }

        public void OnAppearing()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            var currencyId = ApplicationSettings.StartupCurrencyRates;
            var items = UiUtils.Get.Rates.RateItemsFor(currencyId)?.Select(item => new Data(item)).ToList();

            if (items == null) return;

            Device.BeginInvokeOnMainThread(() =>
            {
                _headerClickCallbacks.Clear();
                _currentId = 0;
                _webView.CallJsFunction("setHeader", UiUtils.Get.Rates.SortButtonsFor(currencyId).Select(button => new HeaderData(button, _currentId += 1, this)), string.Empty);
                _webView.CallJsFunction("updateTable", items.ToArray(), CultureInfo.CurrentCulture.Name);
            });
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string Code;
            [DataMember]
            public readonly string Reference;
            [DataMember]
            public readonly string CallbackString;

            public Data(RateItem rateItem)
            {
                Code = rateItem.CurrencyCode;
                Reference = rateItem.FormattedValue;
                CallbackString = rateItem.CurrencyId;
            }
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class HeaderData
        {
            [DataMember]
            public readonly string Text;
            [DataMember]
            public readonly int Id;
            [DataMember]
            public readonly bool? Ascending;

            public HeaderData(SortButtonItem sortButtonItem, int id, RatesTableComponent parent)
            {
                Text = sortButtonItem.Text;
                Id = id;
                Ascending = sortButtonItem.SortAscending;
                parent._headerClickCallbacks[id] = sortButtonItem.OnClick;

            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateView();
        }
    }
}
