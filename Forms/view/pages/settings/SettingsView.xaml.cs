using System;
using MyCryptos.Core.Enums;
using MyCryptos.Core.Settings;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;

namespace MyCryptos.Forms.view.pages.settings
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();

            SetSortCellText();

            AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;
            GraphOnStartup.On = ApplicationSettings.ShowGraphOnStartUp;
            Header.TitleText = I18N.AppName;

            AutoRefresh.Switch.Toggled += AutoRefreshChanged;
            GraphOnStartup.Switch.Toggled += (sender, e) => ApplicationSettings.ShowGraphOnStartUp = e.Value;
            SortingCell.Tapped += (sender, e) => Navigation.PushAsync(new Forms.view.pages.settings.SortSettingsView());
            ReferenceCurrenciesCell.Tapped += (sender, e) => Navigation.PushAsync(new Forms.view.pages.settings.ReferenceCurrenciesSettingsView());
            ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.ReferenceCurrencies);
            SetPinCellText();

            Messaging.Pin.SubscribeValueChanged(this, SetPinCellText);
            Messaging.SortOrder.SubscribeValueChanged(this, SetSortCellText);
            Messaging.ReferenceCurrencies.SubscribeValueChanged(this, () => ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.ReferenceCurrencies));
        }

        private void SetSortCellText()
        {
            var order = string.Empty;
            switch (ApplicationSettings.SortOrder)
            {
                case SortOrder.ALPHABETICAL: order = I18N.Alphabetical; break;
                case SortOrder.BY_UNITS: order = I18N.ByUnits; break;
                case SortOrder.BY_VALUE: order = I18N.ByValue; break;
            }
            var direction = string.Empty;
            switch (ApplicationSettings.SortDirection)
            {
                case SortDirection.ASCENDING: direction = I18N.Ascending; break;
                case SortDirection.DESCENDING: direction = I18N.Descending; break;
            }
            SortingCell.Detail = $"{order} ({direction})";
        }

        private void AutoRefreshChanged(object sender, EventArgs e)
        {
            if (AutoRefresh != null)
            {
                ApplicationSettings.AutoRefreshOnStartup = AutoRefresh.On;
            }
        }

        void PinCellTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Forms.view.pages.settings.PinSettingsView());
        }

        void SetPinCellText()
        {
            PinSettingsCell.Detail = (ApplicationSettings.IsPinSet && ApplicationSettings.IsFingerprintEnabled) ? I18N.FingerprintActive : (ApplicationSettings.IsPinSet) ? I18N.PinActive : I18N.NotConfigured;
        }
    }
}

