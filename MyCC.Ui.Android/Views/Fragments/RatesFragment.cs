﻿using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currency.Model;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Adapter;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class RatesFragment : Fragment
    {
        private Currency _referenceCurrency;
        private List<RateItem> _items;

        private const int RequestCodeCurrency = 1;


        public RatesFragment(Currency referenceCurrency)
        {
            _referenceCurrency = referenceCurrency;
        }

        public RatesFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var saved = savedInstanceState?.GetString("currency");
            if (saved != null)
            {
                _referenceCurrency = JsonConvert.DeserializeObject<Currency>(saved);
            }

            var view = inflater.Inflate(Resource.Layout.fragment_rates, container, false);

            SetVisibleElements(view);

            var headerData = ViewData.Rates.Headers?[_referenceCurrency];
            if (headerData != null)
            {
                var header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
                header.MainText = headerData.MainText;
                header.InfoText = headerData.InfoText;
            }

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.Rates.Send();

            _items = ViewData.Rates.Items?[_referenceCurrency] ?? new List<RateItem>();
            var adapter = new RatesListAdapter(Context, _items);
            view.FindViewById<ListView>(Resource.Id.list_rates).Adapter = adapter;

            var sortData = ViewData.Rates.SortButtons?[_referenceCurrency];
            var sortCurrency = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);
            var sortValue = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            if (sortData != null) SetSortButtons(sortData, sortCurrency, sortValue);


            Messaging.UiUpdate.RatesOverview.Subscribe(this, () =>
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    _items = ViewData.Rates.Items[_referenceCurrency];
                    SetSortButtons(ViewData.Rates.SortButtons?[_referenceCurrency], sortCurrency, sortValue);
                    adapter.Clear();
                    adapter.AddAll(_items);
                    SetVisibleElements(view);
                    refreshView.Refreshing = false;
                });
            });

            view.FindViewById<FloatingActionButton>(Resource.Id.button_add).Click += (sender, args) =>
            {
                var intent = new Intent(Context, typeof(CurrencyPickerActivity));
                intent.PutExtra(CurrencyPickerActivity.ExtraWithoutAlreadyAddedCurrencies, true);
                StartActivityForResult(intent, RequestCodeCurrency);
            };


            return view;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == RequestCodeCurrency && resultCode == (int)Result.Ok)
            {
                var currency = JsonConvert.DeserializeObject<Currency>(data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency));
                ApplicationSettings.WatchedCurrencies = new List<Currency>(ApplicationSettings.WatchedCurrencies) { currency };
                Messaging.Update.Rates.Send();
            }
        }

        private static void SetVisibleElements(View view)
        {
            var data = ViewData.Rates.IsDataAvailable;
            view.FindViewById(Resource.Id.sort_buttons).Visibility = data ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.swiperefresh).Visibility = data ? ViewStates.Visible : ViewStates.Invisible;
            view.FindViewById(Resource.Id.no_data_text).Visibility = data ? ViewStates.Gone : ViewStates.Visible;
        }

        private static void SetSortButtons(IReadOnlyList<SortButtonItem> sortData, SortButtonFragment sortCurrency, SortButtonFragment sortValue)
        {
            sortCurrency.Data = sortData[0];
            sortValue.Data = sortData[1];
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("currency", JsonConvert.SerializeObject(_referenceCurrency));
        }
    }
}