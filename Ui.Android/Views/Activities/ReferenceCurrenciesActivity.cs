using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currencies;
using MyCC.Core.Currencies.Models;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;

namespace MyCC.Ui.Android.Views.Activities
{
    [Activity(Theme = "@style/MyCC")]
    public class ReferenceCurrencyActivity : MyccActivity
    {
        private const int RequestCodeCurrency = 1;
        private LinearLayout _container;
        private HeaderFragment _header;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_reference_currencies);
            SupportActionBar.Title = Resources.GetString(Resource.String.ReferenceCurrencies);

            _header = (HeaderFragment)SupportFragmentManager.FindFragmentById(Resource.Id.header_fragment);
            _container = FindViewById<LinearLayout>(Resource.Id.container_items);
            FindViewById(Resource.Id.button_add).Click += (sender, args) =>
            {
                var intent = new Intent(this, typeof(CurrencyPickerActivity));
                intent.PutExtra(CurrencyPickerActivity.ExtraWithoutReferenceCurrencies, true);
                StartActivityForResult(intent, RequestCodeCurrency);
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RequestCodeCurrency && resultCode == Result.Ok)
            {
                var currencyId = data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency);
                UiUtils.Edit.AddReferenceCurrency(currencyId);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            _header.InfoText = ApplicationSettings.AllReferenceCurrencies.Count().GetPlural(Resource.String.NoCurrencies, Resource.String.OneCurrency, Resource.String.Currencies);

            _container.RemoveAllViews();

            foreach (var c in ApplicationSettings.AllReferenceCurrencies.Select(CurrencyHelper.Find).OrderBy(c => c.Code))
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_reference_currency, null);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = c.Code;
                v.FindViewById<TextView>(Resource.Id.text_info).Text = c.Name;

                var starImage = v.FindViewById<ImageView>(Resource.Id.image_star);
                starImage.SetImageResource(ApplicationSettings.MainCurrencies.Contains(c.Id) ? Resource.Drawable.ic_star_filled : Resource.Drawable.ic_star_empty);

                starImage.Click += (sender, args) =>
                {
                    var isNowMainCurrency = UiUtils.Edit.ToggleReferenceCurrencyStar(c.Id);
                    starImage.SetImageResource(isNowMainCurrency ? Resource.Drawable.ic_star_filled : Resource.Drawable.ic_star_empty);
                };

                v.FindViewById<ImageView>(Resource.Id.image_remove).Click += (sender, args) =>
                {
                    var removed = UiUtils.Edit.RemoveReferenceCurrency(c.Id);
                    if (removed) v.Visibility = ViewStates.Gone;
                };

                _container.AddView(v);
            }
        }
    }
}