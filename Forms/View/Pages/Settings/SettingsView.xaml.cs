using System;
using MyCC.Core.Account.Storage;
using MyCC.Core.Rates;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.Helpers;
using MyCC.Forms.Resources;
using MyCC.Forms.View.Components.BaseComponents;
using MyCC.Forms.View.Pages.Settings.Data;
using MyCC.Forms.View.Pages.Settings.Source;

namespace MyCC.Forms.View.Pages.Settings
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();

            SetDefaultPageCellText();

            AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;

            /*RoundNumbers.On = ApplicationSettings.RoundMoney;
            RoundNumbers.Switch.Toggled += RoundNumbersChanged;*/

            Header.TitleText = I18N.AppName;

            AutoRefresh.Switch.Toggled += AutoRefreshChanged;
            ReferenceCurrenciesCell.Tapped += (sender, e) => Navigation.PushAsync(new ReferenceCurrenciesSettingsView());
            ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.AllReferenceCurrencies);
            SourcesCell.Tapped += (sender, e) => Navigation.PushAsync(new SourcesView());
            SourcesCell.Detail = PluralHelper.GetTextAccounts(AccountStorage.Instance.AllElements.Count);
            RatesCell.Tapped += (sender, e) => Navigation.PushAsync(new WatchedCurrenciesSettingsView());
            PreferredRateCell.Tapped += (s, e) => Navigation.PushAsync(new PreferredBitcoinSettingsPage());
            PrivacyCell.Tapped += (sender, e) => Navigation.PushAsync(new WebContentView("https://www.iubenda.com/privacy-policy/8085117"));
            RatesCell.DetailBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            /*
                        AvailableCurrenciesCell.Tapped += (sender, args) => Navigation.PushAsync(new CurrencyGroupedInfoView());
            */
            SetRatesCellDetail();
            SetPinCellText();
            SetAboutCell();
            SetAvailableCurrenciesCell();
            SetPreferredRateCellDetail();
        }

        private void SetDefaultPageCellText()
        {
            DefaultViewCell.Detail = ApplicationSettings.DefaultPage == StartupPage.TableView ? $"{I18N.Assets} ({I18N.Table})" : ApplicationSettings.DefaultPage == StartupPage.GraphView ? $"{I18N.Assets} ({I18N.Graph})" : I18N.Rates;
        }

        private void OpenDefaultViewPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new General.DefaultPageSettingsView());
        }

        private void AutoRefreshChanged(object sender, EventArgs e)
        {
            if (AutoRefresh != null)
            {
                ApplicationSettings.AutoRefreshOnStartup = AutoRefresh.On;
            }
        }

        /* private void RoundNumbersChanged(object sender, EventArgs e)
         {
             ApplicationSettings.RoundMoney = RoundNumbers.On;
             Messaging.RoundNumbers.SendValueChanged();
         }*/

        private void PinCellTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new General.PinSettingsView());
        }

        private void SetPinCellText()
        {
            PinSettingsCell.Detail = ApplicationSettings.IsPinSet && ApplicationSettings.IsFingerprintEnabled ? I18N.FingerprintActive : ApplicationSettings.IsPinSet ? I18N.PinActive : I18N.NotConfigured;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SourcesCell.Detail = PluralHelper.GetTextAccounts(AccountStorage.Instance.AllElements.Count);
            SetRatesCellDetail();
            ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.AllReferenceCurrencies);
            SetDefaultPageCellText();
            SetPinCellText();
            SetPreferredRateCellDetail();
            SetAvailableCurrenciesCell();
        }

        private void SetRatesCellDetail()
        {
            RatesCell.Detail = ApplicationSettings.WatchedCurrencies.Count == 0 ? I18N.None : string.Join(", ", ApplicationSettings.WatchedCurrencies);
        }

        private void SetPreferredRateCellDetail()
        {
            PreferredRateCell.Detail = ExchangeRatesStorage.PreferredBtcRepository.Name;
        }

        private void SetAboutCell()
        {
            AboutCell.Detail = $"{I18N.AppName} - {I18N.Version} {Core.Settings.Constants.AppVersion}";
            AboutCell.Tapped += (sender, e) => Navigation.PushAsync(new WebContentView("Html/about.html", true));
        }

        private void SetAvailableCurrenciesCell()
        {
            /*
                        AvailableCurrenciesCell.Detail = PluralHelper.GetTextCurrencies(CurrencyStorage.Instance.AllElements.Count);
            */
        }
    }
}

