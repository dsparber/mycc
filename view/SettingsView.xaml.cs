using data.settings;
using enums;
using message;
using models;
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
		}

		void ReferenceCurrencyEntered(object sender, System.EventArgs e)
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
			else
			{
				ApplicationSettings.SortOrder = SortOrder.BY_VALUE;
			}
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.FAST), MessageConstants.UpdateCoinsView);
			MessagingCenter.Send(new FetchSpeed(FetchSpeedEnum.FAST), MessageConstants.UpdateAccountsView);
		}
	}
}

