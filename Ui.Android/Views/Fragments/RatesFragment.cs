using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currencies.Model;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Adapter;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using MyCC.Ui.ViewData;
using Newtonsoft.Json;
using Debug = System.Diagnostics.Debug;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class RatesFragment : Fragment
    {
        private Currency _referenceCurrency;
        private List<RateItem> _items;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;


        private const int RequestCodeCurrency = 1;

        private RatesListAdapter _adapter;


        public RatesFragment(Currency referenceCurrency)
        {
            _referenceCurrency = referenceCurrency;
        }

        public RatesFragment() { }

        private bool _editingEnabled;

        public bool EditingEnabled
        {
            get { return _editingEnabled; }
            set
            {
                if (_adapter == null) return;
                _editingEnabled = value;
                _adapter.EditingEnabled = value;
            }
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var saved = savedInstanceState?.GetString("currency");
            if (saved != null)
            {
                _referenceCurrency = JsonConvert.DeserializeObject<Currency>(saved);
            }

            var view = inflater.Inflate(Resource.Layout.fragment_rates, container, false);

            SetVisibleElements(view);

            var headerData = ViewData.ViewData.Rates.Headers?[_referenceCurrency];
            _header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = headerData;

            _footerFragment = (FooterFragment)ChildFragmentManager.FindFragmentById(Resource.Id.footer_fragment);
            _footerFragment.LastUpdate = ViewData.ViewData.Rates.LastUpdate?[_referenceCurrency] ?? DateTime.MinValue;

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.AllRates.Send();

            _items = RatesViewData.Items[_referenceCurrency];
            _adapter = new RatesListAdapter(Context, _items)
            {

                CurrencyRemoved = () =>
                {
                    if (Activity == null) return;
                    Activity?.RunOnUiThread(() =>
                    {
                        if (!RatesViewData.Items.TryGetValue(_referenceCurrency, out _items) || _adapter == null) return;
                        _adapter.Clear();
                        _adapter.AddAll(_items);
                        Messaging.UiUpdate.RatesOverview.Send();
                    });
                }
            };

            var list = view.FindViewById<ListView>(Resource.Id.list_rates);
            list.Adapter = _adapter;
            list.ItemClick += (sender, args) =>
            {
                if (_editingEnabled) return;

                var currency = _adapter.GetItem(args.Position).Currency;

                var intent = new Intent(Activity, typeof(CoinInfoActivity));
                intent.PutExtra(CoinInfoActivity.ExtraCurrency, JsonConvert.SerializeObject(currency));
                intent.PutExtra(CoinInfoActivity.ExtraShowAccountsButton, true);
                StartActivity(intent);
            };

            var sortData = ViewData.ViewData.Rates.SortButtons?[_referenceCurrency];
            var sortCurrency = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);
            var sortValue = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            if (sortData != null) SetSortButtons(sortData, sortCurrency, sortValue);

            EditingEnabled = MainActivity.RatesEditingEnabled;

            Messaging.UiUpdate.RatesOverview.Subscribe(this, () =>
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                            {
                                // ReSharper disable once RedundantAssignment
                                var lastUpdate = DateTime.Now;
                                if (!ViewData.ViewData.Rates.IsDataAvailable) return;
                                if (!ApplicationSettings.MainCurrencies.Contains(_referenceCurrency.Id)) return;
                                if (!ViewData.ViewData.Rates.Headers?.TryGetValue(_referenceCurrency, out headerData) ?? false) return;
                                if (!ViewData.ViewData.Rates.LastUpdate?.TryGetValue(_referenceCurrency, out lastUpdate) ?? false) return;

                                _header.Data = headerData;
                                _footerFragment.LastUpdate = lastUpdate;
                                _items = RatesViewData.Items[_referenceCurrency];
                                SetSortButtons(ViewData.ViewData.Rates.SortButtons?[_referenceCurrency], sortCurrency, sortValue);
                                _adapter.Clear();
                                _adapter.AddAll(_items);
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

            var activityRootView = view.FindViewById(Resource.Id.fragment_root);

            activityRootView.ViewTreeObserver.GlobalLayout += (sender, args) =>
                        {
                            if (IsDetached || !IsAdded) return;
                            ChildFragmentManager.SetFragmentVisibility(_header, activityRootView.Height > 360.DpToPx());
                        };

            return view;
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (_adapter == null) return;

            _items = RatesViewData.Items[_referenceCurrency];
            _adapter.Clear();
            _adapter.AddAll(_items);
            EditingEnabled = MainActivity.RatesEditingEnabled;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == RequestCodeCurrency && resultCode == (int)Result.Ok)
            {
                var currency = JsonConvert.DeserializeObject<Currency>(data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency));
                CurrencySettingsData.Add(currency);
                Messaging.Update.Rates.Send();
            }
        }

        private static void SetVisibleElements(View view)
        {
            var data = ViewData.ViewData.Rates.IsDataAvailable;
            view.FindViewById(Resource.Id.sort_buttons).Visibility = data ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.swiperefresh).Visibility = data ? ViewStates.Visible : ViewStates.Invisible;
            view.FindViewById(Resource.Id.no_data_text).Visibility = data ? ViewStates.Gone : ViewStates.Visible;
        }

        private static void SetSortButtons(IReadOnlyList<SortButtonItem> sortData, SortButtonFragment sortCurrency, SortButtonFragment sortValue)
        {
            if (sortData == null) return;
            sortCurrency.Data = sortData[0];
            sortCurrency.First = true;
            sortValue.Data = sortData[1];
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("currency", JsonConvert.SerializeObject(_referenceCurrency));
        }
    }
}