using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Ui.Android.Views.Adapter;
using Newtonsoft.Json;
using MyCC.Core.Settings;
using MyCC.Ui.ViewData;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class CurrencyPickerActivity : MyccActivity
    {
        public const string ExtraOnlyAddressCurrencies = "onlyAddressCurrencies";
        public const string ExtraCurrency = "currency";
        public const string ExtraWithoutAlreadyAddedCurrencies = "withoutAlreadyAddedCurrencies";
        public const string ExtraWithoutReferenceCurrencies = "withoutReferenceCurrencies";

        private ListView _listView;
        private SearchView _searchView;
        private ProgressBar _progressBar;
        private TextView _loadingTextView;
        private CurrencyListAdapter _adapter;
        private List<Currency> _currencies;
        private List<Currency> _currenciesForAdapter;

        private bool _onlyAddressCurrencies;
        private bool _withoutAlreadyAddedCurrencies;
        private bool _withoutReferenceCurrencies;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_currency_picker);

            SupportActionBar.Elevation = 3;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _onlyAddressCurrencies = Intent?.GetBooleanExtra(ExtraOnlyAddressCurrencies, false) ?? false;
            _withoutAlreadyAddedCurrencies = Intent?.GetBooleanExtra(ExtraWithoutAlreadyAddedCurrencies, false) ?? false;
            _withoutReferenceCurrencies = Intent?.GetBooleanExtra(ExtraWithoutReferenceCurrencies, false) ?? false;

            _listView = FindViewById<ListView>(Resource.Id.list_currencies);
            _searchView = FindViewById<SearchView>(Resource.Id.search);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progress_bar);
            _loadingTextView = FindViewById<TextView>(Resource.Id.text_loading_currencies);

            _currencies = new List<Currency>();
            _currenciesForAdapter = new List<Currency>();
            _adapter = new CurrencyListAdapter(this, _currenciesForAdapter);

            _listView.Adapter = _adapter;
            _listView.ItemClick += SelectionCallback;

            _searchView.QueryTextChange += FilterCurrencies;
            _searchView.SetIconifiedByDefault(false);
            _searchView.Click += (sender, args) => _searchView.Iconified = false;

            Task.Run(() => FillListView());
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void FilterCurrencies(object sender, SearchView.QueryTextChangeEventArgs queryTextChangeEventArgs)
        {
            _progressBar.Visibility = ViewStates.Visible;
            Task.Run(() => FilterListView(queryTextChangeEventArgs.NewText));
        }

        private void SelectionCallback(object sender, AdapterView.ItemClickEventArgs e)
        {
            var resultData = new Intent();
            resultData.PutExtra(ExtraCurrency, JsonConvert.SerializeObject(_adapter.GetItem(e.Position)));
            SetResult(Result.Ok, resultData);
            Finish();
        }

        private void FilterListView(string text)
        {
            List<Currency> currencies;

            if (!string.IsNullOrWhiteSpace(text))
            {
                var lower = text.ToLower();
                currencies = _currencies?.Where(c => c != null && $"{c.Code}_{c.Name}".ToLower().Contains(lower)).ToList();
            }
            else
            {
                currencies = _currencies;
            }

            RunOnUiThread(() =>
            {
                _adapter.Clear();
                if (currencies != null && currencies.Any())
                {
                    _adapter.AddAll(currencies);
                }
                _adapter.NotifyDataSetChanged();
                _progressBar.Visibility = ViewStates.Gone;
            });
        }

        private void FillListView()
        {
            var exceptions = _withoutAlreadyAddedCurrencies ? CurrencySettingsData.EnabledCurrencies : new List<Currency>();
            exceptions = exceptions.Concat(_withoutReferenceCurrencies ? ApplicationSettings.AllReferenceCurrencies.Select(id => id.ToCurrency()) : new List<Currency>()).ToList();
            var allCurrencies = (_onlyAddressCurrencies ? AddressAccountRepository.AllSupportedCurrencies : CurrencyStorage.Instance.Currencies).Where(c => c != null).ToList();
            _currencies = allCurrencies.Except(exceptions).OrderBy(c => $"{c.Code} {c.Name}").ToList();

            RunOnUiThread(() =>
            {
                _adapter.AddAll(_currencies);
                _adapter.NotifyDataSetChanged();
                _progressBar.Visibility = ViewStates.Gone;
                _loadingTextView.Visibility = ViewStates.Gone;
                _searchView.RequestFocusFromTouch();
            });
        }
    }
}