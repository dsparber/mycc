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
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Adapter;
using MyCC.Ui.DataItems;
using MyCC.Ui.Messages;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class AssetsTableFragment : Fragment
    {
        private string _referenceCurrencyId;
        private List<AssetItem> _items;
        private HeaderFragment _header;
        private FooterFragment _footerFragment;

        private SortButtonFragment _sortValue;
        private SortButtonFragment _sortAmount;
        private View _fragmentRootView;

        public AssetsTableFragment(string referenceCurrencyId)
        {
            _referenceCurrencyId = referenceCurrencyId;
        }

        public AssetsTableFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _referenceCurrencyId = savedInstanceState?.GetString("currency");

            var view = inflater.Inflate(Resource.Layout.fragment_assets_table, container, false);

            SetVisibleElements(view);

            var headerData = UiUtils.Get.Assets.HeaderFor(_referenceCurrencyId);
            _header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _header.Data = headerData;

            _footerFragment = (FooterFragment)ChildFragmentManager.FindFragmentById(Resource.Id.footer_fragment);
            _footerFragment.LastUpdate = UiUtils.Get.Assets.LastUpdate;

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => UiUtils.Update.FetchAllAssetsAndRates();

            _items = UiUtils.Get.Assets.TableItemsFor(_referenceCurrencyId);
            var adapter = new AssetsListAdapter(Context, _items);
            var list = view.FindViewById<ListView>(Resource.Id.list_assets);
            list.Adapter = adapter;
            list.ItemClick += (sender, args) =>
            {
                var currencyId = adapter.GetItem(args.Position).CurrencyId;

                var accounts = AccountStorage.AccountsWithCurrency(currencyId);

                if (accounts.Count == 1)
                {
                    var intent = new Intent(Activity, typeof(AccountDetailActivity));
                    intent.PutExtra(AccountDetailActivity.ExtraAccountId, accounts.First().Id);
                    StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(Activity, typeof(AccountGroupActivity));
                    intent.PutExtra(AccountGroupActivity.ExtraCurrencyId, currencyId);
                    StartActivity(intent);
                }
            };

            var sortData = UiUtils.Get.Assets.SortButtonsFor(_referenceCurrencyId);
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
                    _header.Data = UiUtils.Get.Assets.HeaderFor(_referenceCurrencyId);
                    _footerFragment.LastUpdate = UiUtils.Get.Assets.LastUpdate;
                    _items = UiUtils.Get.Assets.TableItemsFor(_referenceCurrencyId);
                    SetSortButtons(UiUtils.Get.Assets.SortButtonsFor(_referenceCurrencyId), sortCurrency, _sortAmount, _sortValue);
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
            var data = UiUtils.Get.Assets.IsDataAvailable;
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
            outState.PutString("currency", _referenceCurrencyId);
        }
    }
}