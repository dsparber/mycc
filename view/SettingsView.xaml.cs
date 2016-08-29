using data.settings;
using enums;
using message;
using models;
using System;
using Xamarin.Forms;

namespace view
{
	public partial class SettingsView : ContentPage
	{
		public SettingsView()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			ReferenceCurrency.Text = ApplicationSettings.BaseCurrency.Code;
			AutoRefresh.On = ApplicationSettings.AutoRefreshOnStartup;

			updateCheckmarks();
		}

		void ReferenceCurrencyEntered(object sender, EventArgs e)
		{
			var code = ReferenceCurrency.Text;
			ApplicationSettings.BaseCurrency = new Currency(code);
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.MEDIUM), MessageConstants.UpdateCoinsView);
		}

		void SortOrderSelected(object sender, System.EventArgs e)
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

			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.FAST), MessageConstants.UpdateCoinsView);
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.FAST), MessageConstants.UpdateAccountsView);
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

