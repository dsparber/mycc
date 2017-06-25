using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using MyCC.Core;
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
        private Dictionary<string, TextView> _views;
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

            _views = MyccUtil.Rates.CryptoToFiatSourcesWithDetail.ToList().OrderBy(tuple => tuple.name).ToDictionary(t => t.name, tuple =>
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_bitcoin_exchange, null);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = tuple.name;
                var detailText = v.FindViewById<TextView>(Resource.Id.text_info);
                detailText.Text = tuple.detail;

                var radioButton = v.FindViewById<RadioButton>(Resource.Id.radio_button_selected);
                radioButton.Checked = tuple.selected;

                v.Click += (sender, args) => radioButton.Toggle();
                radioButton.CheckedChange += (sender, args) =>
                {
                    MyccUtil.Rates.SelectedCryptoToFiatSource = tuple.name;
                    Finish();
                };

                container.AddView(v);
                return detailText;
            });

            var swipeRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            swipeRefresh.Refresh += (sender, args) =>
            {
                UiUtils.Update.FetchCryptoToFiatRates();
            };

            Messaging.Update.CryptoToFiatRates.Subscribe(this, () => RunOnUiThread(() =>
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
            foreach (var entry in MyccUtil.Rates.CryptoToFiatSourcesWithDetail)
            {
                _views[entry.name].Text = entry.detail;
            }
        }
    }
}