using System;
using System.Collections.Generic;
using MyCC.Core.Account.Storage;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCC.Forms.Resources;

namespace MyCryptos.Forms.view.pages.settings
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();

            SetDefaultPageCellText();

            AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;
            RoundNumbers.On = ApplicationSettings.RoundMoney;
            Header.TitleText = I18N.AppName;

            AutoRefresh.Switch.Toggled += AutoRefreshChanged;
            RoundNumbers.Switch.Toggled += RoundNumbersChanged;
            ReferenceCurrenciesCell.Tapped += (sender, e) => Navigation.PushAsync(new MyCC.Forms.view.pages.settings.ReferenceCurrenciesSettingsView());
            ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.AllReferenceCurrencies);
            SourcesCell.Tapped += (sender, e) => Navigation.PushAsync(new SourcesView());
            SourcesCell.Detail = PluralHelper.GetTextAccounts(AccountStorage.Instance.AllElements.Count);
            RatesCell.Tapped += (sender, e) => Navigation.PushAsync(new WatchedCurrenciesSettingsView());
            RatesCell.DetailBreakMode = Xamarin.Forms.LineBreakMode.TailTruncation;
            SetRatesCellDetail();
            SetPinCellText();
        }

        private void SetDefaultPageCellText()
        {
            DefaultViewCell.Detail = ApplicationSettings.DefaultPage == StartupPage.TableView ? I18N.Table : ApplicationSettings.DefaultPage == StartupPage.GraphView ? I18N.Graph : I18N.Rates;
        }

        private void OpenDefaultViewPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DefaultPageSettingsView());
        }

        private void AutoRefreshChanged(object sender, EventArgs e)
        {
            if (AutoRefresh != null)
            {
                ApplicationSettings.AutoRefreshOnStartup = AutoRefresh.On;
            }
        }

        private void RoundNumbersChanged(object sender, EventArgs e)
        {
            ApplicationSettings.RoundMoney = RoundNumbers.On;
            Messaging.RoundNumbers.SendValueChanged();
        }

        private void PinCellTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PinSettingsView());
        }

        private void SetPinCellText()
        {
            PinSettingsCell.Detail = (ApplicationSettings.IsPinSet && ApplicationSettings.IsFingerprintEnabled) ? I18N.FingerprintActive : (ApplicationSettings.IsPinSet) ? I18N.PinActive : I18N.NotConfigured;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SourcesCell.Detail = PluralHelper.GetTextAccounts(AccountStorage.Instance.AllElements.Count);
            SetRatesCellDetail();
            ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.AllReferenceCurrencies);
            SetDefaultPageCellText();
            SetPinCellText();
        }

        void SetRatesCellDetail()
        {
            RatesCell.Detail = ApplicationSettings.WatchedCurrencies.Count == 0 ? I18N.AllCurrenciesFromAccounts : string.Join(" + ", new List<string> { I18N.AllCurrenciesFromAccounts, string.Join(", ", ApplicationSettings.WatchedCurrencies) });
        }
    }
}

