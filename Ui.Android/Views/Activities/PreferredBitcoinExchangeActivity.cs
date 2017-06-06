using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using MyCC.Core;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Rates.Models;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;
using MyCC.Ui.Messages;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class PreferredBitcoinExchangeActivity : MyccActivity
    {
        private HeaderFragment _header;
        private FooterFragment _footer;
        private List<Tuple<TextView, string>> _views;
        private static bool _triedUpdate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_preferred_bitcoin_exchange);
            SupportActionBar.Title = Resources.GetString(Resource.String.PreferredBitcoinRate);

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footer = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);

            _header.InfoText = MyccUtil.Rates.CryptoToFiatSourceCount.GetPlural(Resource.String.NoSources, Resource.String.OneSource, Resource.String.Sources);
            SetFooter();

            var container = FindViewById<LinearLayout>(Resource.Id.container_items);

            _views = MyccUtil.Rates.CryptoToFiatSourcesWithRates.ToList().OrderBy(tuple => tuple.name).Select(tuple =>
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_bitcoin_exchange, null);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = tuple.name;
                var detailText = v.FindViewById<TextView>(Resource.Id.text_info);
                detailText.Text = GetDetail(tuple.rates.ToList());

                var radioButton = v.FindViewById<RadioButton>(Resource.Id.radio_button_selected);
                radioButton.Checked = tuple.name == MyccUtil.Rates.SelectedCryptoToFiatSource;

                v.Click += (sender, args) => radioButton.Toggle();
                radioButton.CheckedChange += (sender, args) =>
                {
                    MyccUtil.Rates.SelectedCryptoToFiatSource = tuple.name;
                    Finish();
                };

                container.AddView(v);
                return Tuple.Create(detailText, tuple.name);
            }).ToList();

            var swipeRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            swipeRefresh.Refresh += (sender, args) =>
            {
                UiUtils.Update.FetchCryptoToFiatRates();
            };

            Messaging.UiUpdate.BitcoinExchangeSources.Subscribe(this, () => RunOnUiThread(() =>
            {
                SetFooter();
                UpdateText();
                swipeRefresh.Refreshing = false;
            }));
        }

        private void SetFooter()
        {
            var lastUpdate = MyccUtil.Rates.LastCryptoToFiatUpdate();
            if (lastUpdate == DateTime.MinValue && !_triedUpdate)
            {
                _triedUpdate = true;
                UiUtils.Update.FetchCryptoToFiatRates();
            }
            _footer.LastUpdate = lastUpdate;
        }

        private void UpdateText()
        {
            foreach (var v in _views)
            {
                v.Item1.Text = GetDetail(MyccUtil.Rates.CryptoToFiatSourcesWithRates.First(tuple => tuple.name.Equals(v.Item2)).rates.ToList());
            }
        }

        private static string GetDetail(IReadOnlyCollection<ExchangeRate> rates)
        {
            var usd = rates.FirstOrDefault(rate => rate.Descriptor.Equals(new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id)));
            var eur = rates.FirstOrDefault(rate => rate.Descriptor.Equals(new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Usd.Id)));

            var usdString = new Money(usd?.Rate ?? 0, CurrencyConstants.Usd).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var eurString = new Money(usd?.Rate ?? MyccUtil.Rates.GetRate(new RateDescriptor(CurrencyConstants.Btc.Id, CurrencyConstants.Eur.Id))?.Rate ?? 0, CurrencyConstants.Usd).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var note = eur == null && usd != null ? "*" : string.Empty;

            return $"{eurString}{note} / {usdString}";
        }
    }
}