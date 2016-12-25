using System;
using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;

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
			ReferenceCurrenciesCell.Tapped += (sender, e) => Navigation.PushAsync(new ReferenceCurrenciesSettingsView());
			ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.ReferenceCurrencies);
			SetPinCellText();

			Messaging.Pin.SubscribeValueChanged(this, SetPinCellText);
			Messaging.DefaultView.SubscribeValueChanged(this, SetDefaultPageCellText);
			Messaging.ReferenceCurrencies.SubscribeValueChanged(this, () => ReferenceCurrenciesCell.Detail = string.Join(", ", ApplicationSettings.ReferenceCurrencies));
		}

		private void SetDefaultPageCellText()
		{
			DefaultViewCell.Detail = ApplicationSettings.DefaultPage == StartupPage.TableView ? I18N.Table : I18N.Graph;
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
	}
}

