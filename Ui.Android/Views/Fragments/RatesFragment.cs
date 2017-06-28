using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Adapter;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class RatesFragment : Fragment
    {
        private string _referenceCurrencyId;
        private List<RateItem> _items;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;


        private const int RequestCodeCurrency = 1;

        private RatesListAdapter _adapter;


        public RatesFragment(string referenceCurrencyId)
        {
            _referenceCurrencyId = referenceCurrencyId;
        }

        public RatesFragment() { }

        private bool _editingEnabled;

        public bool EditingEnabled
        {
            get => _editingEnabled;
            set
            {
                if (_adapter == null) return;
                _editingEnabled = value;
                _adapter.EditingEnabled = value;
            }
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _referenceCurrencyId = savedInstanceState?.GetString("currency") ?? _referenceCurrencyId;

            var view = inflater.Inflate(Resource.Layout.fragment_rates, container, false);

            SetVisibleElements(view);

            var headerData = UiUtils.Get.Rates.HeaderFor(_referenceCurrencyId);
            _header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = headerData;

            _footerFragment = (FooterFragment)ChildFragmentManager.FindFragmentById(Resource.Id.footer_fragment);
            _footerFragment.LastUpdate = UiUtils.Get.Rates.LastUpdate;

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => UiUtils.Update.FetchAllRates();

            _items = UiUtils.Get.Rates.RateItemsFor(_referenceCurrencyId);
            _adapter = new RatesListAdapter(Context, _items)
            {

                CurrencyRemoved = () =>
                {
                    if (Activity == null) return;
                    Activity?.RunOnUiThread(() =>
                    {
                        if (!UiUtils.Get.Rates.RateItemsFor(_referenceCurrencyId).Any()) return;
                        _adapter.Clear();
                        _adapter.AddAll(UiUtils.Get.Rates.RateItemsFor(_referenceCurrencyId));
                    });
                }
            };

            var list = view.FindViewById<ListView>(Resource.Id.list_rates);
            list.Adapter = _adapter;
            list.ItemClick += (sender, args) =>
            {
                if (_editingEnabled) return;

                var currencyId = _adapter.GetItem(args.Position).CurrencyId;

                var intent = new Intent(Activity, typeof(CoinInfoActivity));
                intent.PutExtra(CoinInfoActivity.ExtraCurrency, currencyId);
                intent.PutExtra(CoinInfoActivity.ExtraShowAccountsButton, true);
                StartActivity(intent);
            };

            var sortData = UiUtils.Get.Rates.SortButtonsFor(_referenceCurrencyId);
            var sortCurrency = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);
            var sortValue = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            if (sortData != null) SetSortButtons(sortData, sortCurrency, sortValue);

            EditingEnabled = MainActivity.RatesEditingEnabled;

            void Sort()
            {
                _items = UiUtils.Get.Rates.RateItemsFor(_referenceCurrencyId);
                if (_items == null) return;
                SetSortButtons(UiUtils.Get.Rates.SortButtonsFor(_referenceCurrencyId), sortCurrency, sortValue);
                _adapter.Clear();
                _adapter.AddAll(_items);
            }

            void UpdateView()
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    Sort();
                    _header.Data = UiUtils.Get.Rates.HeaderFor(_referenceCurrencyId);
                    _footerFragment.LastUpdate = UiUtils.Get.Rates.LastUpdate;
                    SetVisibleElements(view);
                    refreshView.Refreshing = false;
                });
            }

            Messaging.Update.Rates.Subscribe(this, UpdateView);
            Messaging.Modified.Balances.Subscribe(this, UpdateView);
            Messaging.Modified.WatchedCurrencies.Subscribe(this, UpdateView);
            Messaging.Modified.ReferenceCurrencies.Subscribe(this, UpdateView);
            Messaging.Sort.Rates.Subscribe(this, Sort);

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

            _items = UiUtils.Get.Rates.RateItemsFor(_referenceCurrencyId);
            _adapter.Clear();
            _adapter.AddAll(_items);
            EditingEnabled = MainActivity.RatesEditingEnabled;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == RequestCodeCurrency && resultCode == (int)Result.Ok)
            {
                var currencyId = data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency);
                UiUtils.Edit.AddWatchedCurrency(currencyId);
            }
        }

        private static void SetVisibleElements(View view)
        {
            var data = UiUtils.Get.Rates.IsDataAvailable;
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
            outState.PutString("currency", _referenceCurrencyId);
        }
    }
}