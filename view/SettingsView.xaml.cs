using data.settings;
using enums;
using message;
using models;
using System;
using Xamarin.Forms;
using MyCryptos.view.components;

namespace view
{
	public partial class SettingsView : ContentPage
	{
		CurrencyEntryCell ReferenceCurrency;
		public SettingsView()
		{
			InitializeComponent();

			ReferenceCurrency = new CurrencyEntryCell(Navigation);
			CurrencySection.Add(ReferenceCurrency);
			ReferenceCurrency.SelectedCurrency = ApplicationSettings.BaseCurrency;
			ReferenceCurrency.OnSelected = ReferenceCurrencyEntered;

			AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;
            AutoRefresh.Switch.Toggled += AutoRefreshChanged;
            updateCheckmarks();
		}

		void ReferenceCurrencyEntered(Currency currency)
		{
			ApplicationSettings.BaseCurrency = currency;
			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedReferenceCurrency);
		}

		void SortOrderSelected(object sender, EventArgs e)
		{
			if (sender.Equals(SortOrderAlphabetical))
			{
				ApplicationSettings.SortOrder = SortOrder.ALPHABETICAL;
			}
			else if (sender.Equals(SortOrderByUnits))
			{
				ApplicationSettings.SortOrder = SortOrder.BY_UNITS;
			}
			else if (sender.Equals(SortOrderByValue))
			{
				ApplicationSettings.SortOrder = SortOrder.BY_VALUE;
			}
			else if (sender.Equals(SortDirectionAscending))
			{
				ApplicationSettings.SortDirection = SortDirection.ASCENDING;
			}
			else if (sender.Equals(SortDirectionDescending))
			{
				ApplicationSettings.SortDirection = SortDirection.DESCENDING;
			}
			updateCheckmarks();

			MessagingCenter.Send(string.Empty, MessageConstants.UpdatedSortOrder);
		}

		void AutoRefreshChanged(object sender, EventArgs e)
		{
			if (AutoRefresh != null)
			{
				ApplicationSettings.AutoRefreshOnStartup = AutoRefresh.On;
			}
		}

		void updateCheckmarks()
		{
			SortOrderAlphabetical.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.ALPHABETICAL);
			SortOrderByUnits.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.BY_UNITS);
			SortOrderByValue.ShowIcon = ApplicationSettings.SortOrder.Equals(SortOrder.BY_VALUE);
			SortDirectionAscending.ShowIcon = ApplicationSettings.SortDirection.Equals(SortDirection.ASCENDING);
			SortDirectionDescending.ShowIcon = ApplicationSettings.SortDirection.Equals(SortDirection.DESCENDING);
		}
	}
}

