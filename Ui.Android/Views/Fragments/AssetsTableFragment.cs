﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Storage;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Adapter;
using MyCC.Ui.DataItems;
using MyCC.Ui.Get;
using MyCC.Ui.Messages;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class AssetsTableFragment : Fragment
    {
        private Currency _referenceCurrency;
        private List<AssetItem> _items;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;

        private SortButtonFragment _sortValue;
        private SortButtonFragment _sortAmount;
        private View _fragmentRootView;

        public AssetsTableFragment(Currency referenceCurrency)
        {
            _referenceCurrency = referenceCurrency;
        }

        public AssetsTableFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var saved = savedInstanceState?.GetString("currency");
            if (saved != null)
            {
                _referenceCurrency = JsonConvert.DeserializeObject<Currency>(saved);
            }

            var view = inflater.Inflate(Resource.Layout.fragment_assets_table, container, false);

            SetVisibleElements(view);

            var headerData = ViewData.Assets.Headers?[_referenceCurrency];
            _header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = headerData;

            _footerFragment = (FooterFragment)ChildFragmentManager.FindFragmentById(Resource.Id.footer_fragment);
            _footerFragment.LastUpdate = ViewData.Assets.LastUpdate?[_referenceCurrency] ?? DateTime.MinValue;

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.AllAssetsAndRates.Send();

            _items = ViewData.Assets.Items?[_referenceCurrency] ?? new List<AssetItem>();
            var adapter = new AssetsListAdapter(Context, _items);
            var list = view.FindViewById<ListView>(Resource.Id.list_assets);
            list.Adapter = adapter;
            list.ItemClick += (sender, args) =>
            {
                var currency = adapter.GetItem(args.Position).Value.Currency;

                var accounts = AccountStorage.AccountsWithCurrency(currency);

                if (accounts.Count == 1)
                {
                    var intent = new Intent(Activity, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, accounts.First().Id);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(Activity, typeof(AccountGroupActivity));
                    intent.PutExtra(AccountGroupActivity.ExtraCurrencyId, currency.Id);
                    StartActivity(intent);
                }
            };

            var sortData = ViewData.Assets.SortButtons?[_referenceCurrency];
            var sortCurrency = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);
            _sortAmount = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_amount_sort);
            _sortValue = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            if (sortData != null) SetSortButtons(sortData, sortCurrency, _sortAmount, _sortValue);

            _fragmentRootView = view.FindViewById(Resource.Id.fragment_root);


            Messaging.UiUpdate.AssetsTable.Subscribe(this, () =>
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    if (!ViewData.Assets.IsDataAvailable) return;
                    if (!ApplicationSettings.MainCurrencies.Contains(_referenceCurrency.Id)) return;
                    if (!ViewData.Assets.Headers.TryGetValue(_referenceCurrency, out headerData)) return;

                    _header.Data = headerData;
                    _footerFragment.LastUpdate = ViewData.Assets.LastUpdate[_referenceCurrency];
                    _items = ViewData.Assets.Items[_referenceCurrency];
                    SetSortButtons(ViewData.Assets.SortButtons?[_referenceCurrency], sortCurrency, _sortAmount, _sortValue);
                    adapter.Clear();
                    adapter.AddAll(_items);
                    SetVisibleElements(view);
                    refreshView.Refreshing = false;
                });
            });

            view.FindViewById<FloatingActionButton>(Resource.Id.button_add).Click += (sender, args) =>
            {
                StartActivity(new Intent(Application.Context, typeof(AddSourceActivity)));
            };

            _fragmentRootView.ViewTreeObserver.GlobalLayout += (sender, args) =>
            {
                if (IsDetached || !IsAdded) return;
                ChildFragmentManager.SetFragmentVisibility(_header, _fragmentRootView.Height > 360.DpToPx());
                ChildFragmentManager.SetFragmentVisibility(_sortValue, !(ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Value && _fragmentRootView.Width < 480.DpToPx()));
                ChildFragmentManager.SetFragmentVisibility(_sortAmount, !(ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Amount && _fragmentRootView.Width < 480.DpToPx()));
            };

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            ChildFragmentManager.SetFragmentVisibility(_sortValue, !(ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Value && _fragmentRootView.Width < 480.DpToPx()));
            ChildFragmentManager.SetFragmentVisibility(_sortAmount, !(ApplicationSettings.AssetsColumToHideIfSmall == ColumnToHide.Amount && _fragmentRootView.Width < 480.DpToPx()));
        }

        private static void SetVisibleElements(View view)
        {
            var data = ViewData.Assets.IsDataAvailable;
            view.FindViewById(Resource.Id.sort_buttons).Visibility = data ? ViewStates.Visible : ViewStates.Gone;
            view.FindViewById(Resource.Id.swiperefresh).Visibility = data ? ViewStates.Visible : ViewStates.Invisible;
            view.FindViewById(Resource.Id.no_data_text).Visibility = data ? ViewStates.Gone : ViewStates.Visible;
        }

        private static void SetSortButtons(IReadOnlyList<SortButtonItem> sortData, SortButtonFragment sortCurrency, SortButtonFragment sortAmount, SortButtonFragment sortValue)
        {
            sortCurrency.Data = sortData[0];
            sortCurrency.First = true;
            sortAmount.Data = sortData[1];
            sortValue.Data = sortData[2];
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("currency", JsonConvert.SerializeObject(_referenceCurrency));
        }
    }
}