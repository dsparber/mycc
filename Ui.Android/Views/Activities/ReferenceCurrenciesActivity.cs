using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MyCC.Core.Currency.Model;
using MyCC.Core.Settings;
using MyCC.Ui.Android.Helpers;
using MyCC.Ui.Android.Views.Fragments;
using MyCC.Ui.Messages;
using Newtonsoft.Json;

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
                var currency = JsonConvert.DeserializeObject<Currency>(data.GetStringExtra(CurrencyPickerActivity.ExtraCurrency));
                ApplicationSettings.FurtherCurrencies = new List<Currency>(ApplicationSettings.FurtherCurrencies) { currency };
                Messaging.Update.AllItems.Send();
                Messaging.UiUpdate.ViewsWithRate.Send();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            _header.InfoText = ApplicationSettings.AllReferenceCurrencies.Count.GetPlural(Resource.String.NoCurrencies, Resource.String.OneCurrency, Resource.String.Currencies);

            _container.RemoveAllViews();

            foreach (var c in ApplicationSettings.AllReferenceCurrencies.OrderBy(c => c.Code))
            {
                var v = LayoutInflater.Inflate(Resource.Layout.item_reference_currency, null);
                v.FindViewById<TextView>(Resource.Id.text_name).Text = c.Code;
                v.FindViewById<TextView>(Resource.Id.text_info).Text = c.Name;

                var starImage = v.FindViewById<ImageView>(Resource.Id.image_star);
                starImage.SetImageResource(ApplicationSettings.MainCurrencies.Contains(c) ? Resource.Drawable.ic_star_filled : Resource.Drawable.ic_star_empty);

                starImage.Click += (sender, args) =>
                {
                    var willBecomeMainCurrency = !ApplicationSettings.MainCurrencies.Contains(c);
                    if (willBecomeMainCurrency && ApplicationSettings.MainCurrencies.Count >= 3)
                    {
                        this.ShowInfoDialog(Resource.String.Error, Resource.String.OnlyThreeCurrenciesCanBeStared);
                    }
                    else if (!willBecomeMainCurrency && c.Equals(Currency.Btc))
                    {
                        this.ShowInfoDialog(Resource.String.Error, Resource.String.BitcoinCanNotBeRemoved);
                    }
                    else
                    {
                        starImage.SetImageResource(willBecomeMainCurrency ? Resource.Drawable.ic_star_filled : Resource.Drawable.ic_star_empty);

                        var ca = new[] { c };

                        ApplicationSettings.MainCurrencies = (willBecomeMainCurrency ? ApplicationSettings.MainCurrencies.Concat(ca) : ApplicationSettings.MainCurrencies.Except(ca)).ToList();
                        ApplicationSettings.FurtherCurrencies = (willBecomeMainCurrency ? ApplicationSettings.FurtherCurrencies.Except(ca) : ApplicationSettings.FurtherCurrencies.Concat(ca)).ToList();
                        Messaging.Update.AllItems.Send();
                        Messaging.UiUpdate.ViewsWithRate.Send();
                    }
                };

                v.FindViewById<ImageView>(Resource.Id.image_remove).Click += (sender, args) =>
                {
                    if (c.Equals(Currency.Btc))
                    {
                        this.ShowInfoDialog(Resource.String.Error, Resource.String.BitcoinCanNotBeRemoved);
                    }
                    else
                    {
                        var wasMainCurrency = ApplicationSettings.MainCurrencies.Contains(c);
                        if (wasMainCurrency)
                        {
                            ApplicationSettings.MainCurrencies = ApplicationSettings.MainCurrencies.Except(new[] { c }).ToList();
                        }
                        else
                        {
                            ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Except(new[] { c }).ToList();
                        }
                        v.Visibility = ViewStates.Gone;
                        Messaging.Update.AllItems.Send();
                        Messaging.UiUpdate.ViewsWithRate.Send();
                    }
                };

                _container.AddView(v);
            }
        }
    }
}