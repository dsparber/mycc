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
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Data.Get;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Activities;
using MyCC.Ui.Android.Views.Adapter;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class AssetsTableFragment : Fragment
    {
        private Currency _referenceCurrency;
        private List<AssetItem> _items;

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
            if (headerData != null)
            {
                var header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
                header.MainText = headerData.MainText;
                header.InfoText = headerData.InfoText;
            }

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.Assets.Send();

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
            var sortAmount = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_amount_sort);
            var sortValue = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            if (sortData != null) SetSortButtons(sortData, sortCurrency, sortAmount, sortValue);

            Messaging.UiUpdate.AssetsTable.Subscribe(this, () =>
            {
                if (Activity == null) return;
                Activity.RunOnUiThread(() =>
                {
                    _items = ViewData.Assets.Items[_referenceCurrency];
                    SetSortButtons(ViewData.Assets.SortButtons?[_referenceCurrency], sortCurrency, sortAmount, sortValue);
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

            return view;
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