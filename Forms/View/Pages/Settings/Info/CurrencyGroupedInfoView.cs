using System;
using System.Collections.Generic;
using System.Linq;
using MyCC.Core.Account.Repositories.Base;
using MyCC.Core.CoinInfo;
using MyCC.Core.Currency.Model;
using MyCC.Core.Currency.Repositories;
using MyCC.Core.Currency.Storage;
using MyCC.Core.Rates;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components;
using MyCC.Forms.View.Components.Cells;
using MyCC.Forms.View.Overlays;
using Xamarin.Forms;

namespace MyCC.Forms.View.Pages.Settings.Info
{
    public class CurrencyGroupedInfoView : ContentPage
    {
        public CurrencyGroupedInfoView()
        {
            Title = I18N.AvailableCurrencies;

            var allCurrencies = CurrencyStorage.Instance.AllElements.ToList();
            var fiatCurrencies = allCurrencies.Where(c => !c.IsCryptoCurrency).ToList();
            var cryptoCurrencies = allCurrencies.Where(c => c.IsCryptoCurrency).ToList();

            var bittrexCurrencies = CurrencyStorage.CurrenciesOf<BittrexCurrencyRepository>().ToList();
            var addressCurrencies = AddressAccountRepository.AllSupportedCurrencies.ToList();
            var balanceCurrencies = bittrexCurrencies.Concat(addressCurrencies).Distinct().ToList();

            var coinInfos = CoinInfoStorage.SupportetCurrencies.ToList();
            var cryptoRates = CurrencyStorage.CurrenciesOf<BittrexCurrencyRepository>()
                .Concat(CurrencyStorage.CurrenciesOf<CryptonatorCurrencyRepository>())
                .Concat(CurrencyStorage.CurrenciesOf<BtceCurrencyRepository>())
                .Distinct().ToList();
            var fiatRates = ExchangeRatesStorage.FixerIo.Rates.Select(r => CurrencyStorage.Instance.AllElements.Find(x => Equals(x?.Code, r?.SecondaryCurrencyCode) && x?.IsCryptoCurrency == false)).Distinct().ToList();
            var rates = cryptoRates.Concat(fiatRates).Distinct().ToList();

            var allCurrenciesCell = new CustomViewCell
            {
                Text = PluralHelper.GetTextCurrencies(allCurrencies.Count),
                Image = "more",
            };
            allCurrenciesCell.Tapped += OpenFor(allCurrencies, I18N.AllCurrencies);

            var fiatCurrenciesCell = new CustomViewCell
            {
                Text = $"{fiatCurrencies.Count} {I18N.FiatCurrencies}",
                Image = "more",
            };
            fiatCurrenciesCell.Tapped += OpenFor(fiatCurrencies, I18N.FiatCurrencies);

            var cryptoCurrenciesCell = new CustomViewCell
            {
                Text = $"{cryptoCurrencies.Count} {I18N.Cryptocurrencies}",
                Image = "more",
            };
            cryptoCurrenciesCell.Tapped += OpenFor(cryptoCurrencies, I18N.Cryptocurrencies);

            var balanceCell = new CustomViewCell
            {
                Text = I18N.BalanceAvailable,
                Detail = PluralHelper.GetTextCurrencies(balanceCurrencies.Count),
                Image = "more",
            };
            balanceCell.Tapped += OpenFor(balanceCurrencies, I18N.BalanceAvailable);

            var addressCell = new CustomViewCell
            {
                Text = I18N.BalanceAvailableAddress,
                Detail = PluralHelper.GetTextCurrencies(addressCurrencies.Count),
                Image = "more",
            };
            addressCell.Tapped += OpenFor(addressCurrencies, I18N.BalanceAvailableAddress);

            var bittrexCell = new CustomViewCell
            {
                Text = I18N.BalanceAvailableBittrex,
                Detail = PluralHelper.GetTextCurrencies(bittrexCurrencies.Count),
                Image = "more",
            };
            bittrexCell.Tapped += OpenFor(bittrexCurrencies, I18N.BalanceAvailableBittrex);

            var ratesCell = new CustomViewCell
            {
                Text = I18N.Rates,
                Detail = PluralHelper.GetTextCurrencies(rates.Count),
                Image = "more",
            };
            ratesCell.Tapped += OpenFor(rates, I18N.Rates);

            var infosCell = new CustomViewCell
            {
                Text = I18N.Info,
                Detail = PluralHelper.GetTextCurrencies(coinInfos.Count),
                Image = "more",
            };
            infosCell.Tapped += OpenFor(coinInfos, I18N.Info);


            Content = new ChangingStackLayout
            {
                Children =
                {
                    new HeaderView
                    {
                        TitleText = I18N.AppName,
                        InfoText = PluralHelper.GetTextCurrencies(allCurrencies.Count)
                    },
                    new TableView()
                    {
                        Intent = TableIntent.Form,
                        Root =
                        {
                            new TableSection(I18N.General)
                            {
                                allCurrenciesCell,
                                fiatCurrenciesCell,
                                cryptoCurrenciesCell
                            },
                            new TableSection(I18N.Balance)
                            {
                                balanceCell,
                                addressCell,
                                bittrexCell
                            },
                            new TableSection(I18N.Further)
                            {
                                ratesCell,
                                infosCell
                            }
                        }
                    }
                }
            };
        }

        private EventHandler OpenFor(IEnumerable<Currency> currencies, string title) => (sender, e) => Navigation.PushAsync(new CurrencyOverlay(currencies, title, false, true));

    }
}