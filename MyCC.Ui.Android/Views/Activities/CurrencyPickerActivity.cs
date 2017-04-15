using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Storage;
using MyCC.Ui.Android.Views.Adapter;
using Newtonsoft.Json;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class CurrencyPickerActivity : AppCompatActivity
    {
        private ListView _listView;
        private SearchView _searchView;
        private ProgressBar _progressBar;
        private TextView _loadingTextView;
        private CurrencyListAdapter _adapter;
        private List<Currency> _currencies;
        private List<Currency> _currenciesForAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_currency_picker);

            _listView = FindViewById<ListView>(Resource.Id.list_currencies);
            _searchView = FindViewById<SearchView>(Resource.Id.search);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progress_bar);
            _loadingTextView = FindViewById<TextView>(Resource.Id.text_loading_currencies);

            _currenciesForAdapter = new List<Currency>();
            _adapter = new CurrencyListAdapter(this, _currenciesForAdapter);

            _listView.Adapter = _adapter;
            _listView.ItemClick += SelectionCallback;

            _searchView.QueryTextChange += FilterCurrencies;
            _searchView.SetIconifiedByDefault(false);
            _searchView.Click += (sender, args) => _searchView.Iconified = false;

            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);

            Task.Run(() => FillListView());
        }

        private void FilterCurrencies(object sender, SearchView.QueryTextChangeEventArgs queryTextChangeEventArgs)
        {
            _progressBar.Visibility = ViewStates.Visible;
            Task.Run(() => FilterListView(queryTextChangeEventArgs.NewText));
        }

        private void SelectionCallback(object sender, AdapterView.ItemClickEventArgs e)
        {
            var resultData = new Intent();
            resultData.PutExtra("currency", JsonConvert.SerializeObject(_adapter.GetItem(e.Position)));
            SetResult(Result.Ok, resultData);
            Finish();
        }

        private void FilterListView(string text)
        {
            List<Currency> currencies;

            if (!string.IsNullOrWhiteSpace(text))
            {
                var lower = text.ToLower();
                currencies = _currencies.Where(c => $"{c.Code}_{c.Name}".ToLower().Contains(lower)).ToList();
            }
            else
            {
                currencies = _currencies;
            }

            RunOnUiThread(() =>
            {
                _adapter.Clear();
                _adapter.AddAll(currencies);
                _adapter.NotifyDataSetChanged();
                _progressBar.Visibility = ViewStates.Gone;
            });
        }

        private void FillListView()
        {
            _currencies = CurrencyStorage.Instance.AllElements.OrderBy(c => $"{c.Code} {c.Name}").ToList();

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