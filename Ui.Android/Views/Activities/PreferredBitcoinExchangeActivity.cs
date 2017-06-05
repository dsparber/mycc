using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using MyCC.Core.Account.Models.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Rates;
using MyCC.Core.Rates.Repositories;
using MyCC.Core.Rates.Utils;
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
        private List<Tuple<TextView, IRateSource>> _views;
        private static bool _triedUpdate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_preferred_bitcoin_exchange);
            SupportActionBar.Title = Resources.GetString(Resource.String.PreferredBitcoinRate);

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _footer = (FooterFragment)SupportFragmentManager.FindFragmentById(Resource.Id.footer_fragment);

            _header.InfoText = RateStorage.BitcoinRepositories.Count.GetPlural(Resource.String.NoSources, Resource.String.OneSource, Resource.String.Sources);
            SetFooter();

            var container = FindViewById<LinearLayout>(Resource.Id.container_items);

            _views = RateStorage.BitcoinRepositories.OrderBy(r => r.Name).Select(source =>
            {

                var v = LayoutInflater.Inflate(Resource.Layout.item_bitcoin_exchange, null);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = source.Name;
                var detailText = v.FindViewById<TextView>(Resource.Id.text_info);
                detailText.Text = GetDetail(source).Item1;

                var radioButton = v.FindViewById<RadioButton>(Resource.Id.radio_button_selected);
                radioButton.Checked = source.Id == ApplicationSettings.PreferredBitcoinRepository;

                v.Click += (sender, args) => radioButton.Toggle();
                radioButton.CheckedChange += (sender, args) =>
                {
                    ApplicationSettings.PreferredBitcoinRepository = source.Id;
                    Finish();
                };

                container.AddView(v);
                return Tuple.Create(detailText, source);
            }).ToList();

            var swipeRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            swipeRefresh.Refresh += (sender, args) =>
            {
                Messaging.Request.BitcoinExchangeSources.Send();
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
            var lastUpdate = RateStorage.BitcoinRepositories.Select(r => GetDetail(r).Item2).Min();
            if (lastUpdate == DateTime.MinValue && !_triedUpdate)
            {
                _triedUpdate = true;
                Messaging.Request.BitcoinExchangeSources.Send();
            }
            _footer.LastUpdate = lastUpdate;
        }

        private void UpdateText()
        {
            foreach (var v in _views)
            {
                v.Item1.Text = GetDetail(v.Item2).Item1;
            }
        }

        private static Tuple<string, DateTime> GetDetail(IRateSource source)
        {
            var usd = RateUtil.GetStoredRate(CurrencyConstants.Btc, CurrencyConstants.Usd, source.Id);
            var eur = RateUtil.GetStoredRate(CurrencyConstants.Btc, CurrencyConstants.Eur, source.Id);

            var usdString = (usd?.AsMoney ?? new Money(0, CurrencyConstants.Usd)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var eurString = (eur?.AsMoney ?? RateUtil.GetRate(CurrencyConstants.Btc, CurrencyConstants.Eur, source.Id)?.AsMoney ?? new Money(0, CurrencyConstants.Eur)).ToStringTwoDigits(ApplicationSettings.RoundMoney);
            var note = eur == null && usd != null ? "*" : string.Empty;

            return Tuple.Create($"{eurString}{note} / {usdString}", usd?.LastUpdate ?? eur?.LastUpdate ?? DateTime.MinValue);
        }
    }
}