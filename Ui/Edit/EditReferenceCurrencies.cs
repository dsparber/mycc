using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Currencies;
using MyCC.Core.Settings;
using MyCC.Ui.Helpers;
using MyCC.Ui.Messages;
using Xamarin.Forms;

namespace MyCC.Ui.Edit
{
    internal static class EditReferenceCurrencies
    {
        public static void AddReferenceCurrency(string currencyId)
        {
            ApplicationSettings.FurtherCurrencies = new List<string>(ApplicationSettings.FurtherCurrencies) { currencyId };
            UiUtils.Update.FetchNeededButNotLoadedRates();
        }

        public static bool RemoveReferenceCurrency(string currencyId)
        {
            if (currencyId.Equals(CurrencyConstants.Btc.Id))
            {
                DependencyService.Get<IErrorDialog>().Display(DependencyService.Get<ITextResolver>().BitcoinCanNotBeRemoved);
                return false;
            }

            var wasMainCurrency = ApplicationSettings.MainCurrencies.Contains(currencyId);
            if (wasMainCurrency)
            {
                ApplicationSettings.MainCurrencies = ApplicationSettings.MainCurrencies.Except(new[] { currencyId }).ToList();
                UiUtils.AssetsRefresh.ResetCache();
                UiUtils.RatesRefresh.ResetCache();
            }
            else
            {
                ApplicationSettings.FurtherCurrencies = ApplicationSettings.FurtherCurrencies.Except(new[] { currencyId }).ToList();
            }

            Messaging.Update.Balances.Send();
            Messaging.Update.Rates.Send();
            return true;

        }

        public static bool ToggleReferenceCurrencyStar(string currencyId)
        {
            var willBecomeMainCurrency = !ApplicationSettings.MainCurrencies.Contains(currencyId);
            var isNowMainCurrency = willBecomeMainCurrency;

            if (willBecomeMainCurrency && ApplicationSettings.MainCurrencies.Count() >= 3)
            {
                isNowMainCurrency = false;
                DependencyService.Get<IErrorDialog>().Display(DependencyService.Get<ITextResolver>().OnlyThreeCurrenciesCanBeStared);
            }
            else if (!willBecomeMainCurrency && currencyId.Equals(CurrencyConstants.Btc.Id))
            {
                isNowMainCurrency = true;
                DependencyService.Get<IErrorDialog>().Display(DependencyService.Get<ITextResolver>().BitcoinCanNotBeRemoved);
            }
            else
            {
                var currencyIdAsArray = new[] { currencyId };

                ApplicationSettings.MainCurrencies = (willBecomeMainCurrency ? ApplicationSettings.MainCurrencies.Concat(currencyIdAsArray) : ApplicationSettings.MainCurrencies.Except(currencyIdAsArray)).ToList();
                ApplicationSettings.FurtherCurrencies = (willBecomeMainCurrency ? ApplicationSettings.FurtherCurrencies.Except(currencyIdAsArray) : ApplicationSettings.FurtherCurrencies.Concat(currencyIdAsArray)).ToList();
                UiUtils.AssetsRefresh.ResetCache();
                UiUtils.RatesRefresh.ResetCache();
                Messaging.Update.Rates.Send();
                Messaging.Update.Balances.Send();
            }
            return isNowMainCurrency;
        }
    }
}