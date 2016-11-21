using System;
using data.settings;
using enums;
using message;
using MyCryptos.resources;
using Xamarin.Forms;

namespace MyCryptos.view.pages.settings
{
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();

            SetSortCellText();

            AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;
            Header.TitleText = I18N.AppName;

            AutoRefresh.Switch.Toggled += AutoRefreshChanged;
            SortingCell.Tapped += (sender, e) => Navigation.PushAsync(new SortSettingsView());
            ReferenceCurrenciesCell.Tapped += (sender, e) => Navigation.PushAsync(new ReferenceCurrenciesSettingsView());
            ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.ReferenceCurrencies);

            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedSortOrder, (str) => SetSortCellText());
            MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedReferenceCurrencies, str => ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.ReferenceCurrencies));
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

    }
}

