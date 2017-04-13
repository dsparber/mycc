using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currency.Model;
using MyCC.Ui.Android.Data;
using MyCC.Ui.Android.Messages;
using MyCC.Ui.Android.Views.Adapter;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Fragments
{
    public class RatesFragment : Fragment
    {
        private Currency _referenceCurrency;
        private List<RateItem> _items;

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

            var headerData = ViewData.Rates.RateHeaders?[_referenceCurrency];
            if (headerData != null)
            {
                var header = (HeaderFragment)ChildFragmentManager.FindFragmentById(Resource.Id.header_fragment);
                header.MainText = headerData.MainText;
                header.InfoText = headerData.InfoText;
            }

            var refreshView = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            refreshView.Refresh += (sender, args) => Messaging.Request.Rates.Send();

            _items = ViewData.Rates.RateItems?[_referenceCurrency] ?? new List<RateItem>();
            var adapter = new RatesListAdapter(Context, _items);
            view.FindViewById<ListView>(Resource.Id.list_rates).Adapter = adapter;

            var sortData = ViewData.Rates.RateSortButtons?[_referenceCurrency];
            var sortCurrency = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_currency_sort);
            var sortValue = (SortButtonFragment)ChildFragmentManager.FindFragmentById(Resource.Id.button_value_sort);
            if (sortData != null) SetSortButtons(sortData, sortCurrency, sortValue);


            Messaging.UiUpdate.RatesOverview.Subscribe(this, () =>
            {
                _items = ViewData.Rates.RateItems[_referenceCurrency];
                SetSortButtons(ViewData.Rates.RateSortButtons?[_referenceCurrency], sortCurrency, sortValue);
                adapter.Clear();
                adapter.AddAll(_items);
                refreshView.Refreshing = false;
            });


            return view;
        }

        private static void SetSortButtons(IReadOnlyList<SortButtonItem> sortData, SortButtonFragment sortCurrency, SortButtonFragment sortValue)
        {
            sortCurrency.Text = sortData[0].Text;
            sortCurrency.Gravity = sortData[0].TextGravity;
            sortCurrency.Direction = sortData[0].SortDirection;
            sortCurrency.OnClick = sortData[0].OnClick;

            sortValue.Text = sortData[1].Text;
            sortValue.Gravity = sortData[1].TextGravity;
            sortValue.Direction = sortData[1].SortDirection;
            sortValue.OnClick = sortData[1].OnClick;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("currency", JsonConvert.SerializeObject(_referenceCurrency));
        }
    }
}