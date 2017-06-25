using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using MyCC.Core.Account.Storage;
using MyCC.Core.Settings;
using MyCC.Forms.View.Pages;
using Xamarin.Forms;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Ui;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;

namespace MyCC.Forms.View.Components.Table
{
    public class CoinTableComponent : ContentView
    {
        private readonly HybridWebView _webView;
        private readonly Dictionary<int, Action> _headerClickCallbacks;
        private static int _currentId;

        public CoinTableComponent()
        {
            _webView = new HybridWebView("Html/coinTable.html") { LoadFinished = UpdateView };
            _headerClickCallbacks = new Dictionary<int, Action>();

            _webView.RegisterCallback("CallbackSizeAllocated", sizeString =>
            {
                var size = int.Parse(sizeString);
                Device.BeginInvokeOnMainThread(() => _webView.HeightRequest = size);
            });
            _webView.RegisterCallback("Callback", currencyId =>
            {
                var accounts = AccountStorage.AccountsWithCurrency(currencyId);
                var view = accounts.Count == 1 ? new AccountView(accounts[0]) : new AccountGroupView(currencyId) as Page;

                Device.BeginInvokeOnMainThread(async () =>
                {
                    foreach (var page in Navigation.NavigationStack.Where(p => !(p is AssetsTableView))) Navigation.RemovePage(page);
                    await Navigation.PushAsync(view);
                });
            });

            _webView.RegisterCallback("HeaderClickedCallback", id =>
            {
                _headerClickCallbacks[int.Parse(id)].Invoke();
                UpdateView();
            });

            Content = _webView;

            _webView.LoadFinished = UpdateView;

            Messaging.Update.Balances.Subscribe(this, UpdateView);
			Messaging.Update.Rates.Subscribe(this, UpdateView);
            Messaging.Status.CarouselPosition.Subscribe(this, UpdateView);
        }

        private void UpdateView()
        {
            var currencyId = ApplicationSettings.StartupCurrencyAssets;
            var items = UiUtils.Get.Assets.TableItemsFor(currencyId)?.Select(item => new Data(item)).ToList();

            if (items == null) return;

            Device.BeginInvokeOnMainThread(() =>
            {
                _headerClickCallbacks.Clear();
                _webView.CallJsFunction("setHeader", UiUtils.Get.Assets.SortButtonsFor(currencyId).Select(button => new HeaderData(button, _currentId += 1, this)), string.Empty);
                _webView.CallJsFunction("updateTable", items.ToArray(), CultureInfo.CurrentCulture.Name);
            });
        }

        [DataContract]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "NotAccessedField.Global")]
        public class Data
        {
            [DataMember]
            public readonly string CallbackString;
            [DataMember]
            public readonly string Code;
            [DataMember]
            public readonly string Amount;
            [DataMember]
            public readonly string Reference;
            [DataMember]
            public readonly bool Disabled;

            public Data(AssetItem assetItem)
            {
                Code = assetItem.CurrencyCode;
                Amount = assetItem.FormattedValue;
                Reference = assetItem.FormattedReferenceValue;
                CallbackString = assetItem.CurrencyId;
                Disabled = !assetItem.Enabled;
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

            public HeaderData(SortButtonItem sortButtonItem, int id, CoinTableComponent parent)
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
