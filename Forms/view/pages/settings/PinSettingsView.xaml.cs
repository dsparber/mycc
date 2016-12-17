using System;
using MyCryptos.Core.settings;
using MyCryptos.Core.Types;
using MyCryptos.Forms.helpers;
using MyCryptos.Forms.Messages;
using MyCryptos.Forms.Resources;
using MyCryptos.Forms.view.overlays;
using Plugin.Fingerprint;

namespace MyCryptos.Forms.view.pages.settings
{
	public partial class PinSettingsView
	{
		public PinSettingsView()
		{
			InitializeComponent();

			SetPinCells();

			FingerprintCell.Switch.Toggled += (sender, e) => { ApplicationSettings.IsFingerprintEnabled = e.Value; Messaging.Pin.SendValueChanged(); };

			if (ApplicationSettings.IsFingerprintEnabled)
			{
				FingerprintCell.Switch.IsToggled = true;
			}
			else
			{
				Table.Root.Remove(FingerprintSection);
			}

			Messaging.Pin.SubscribeValueChanged(this, SetPinCells);
		}

		private void SetPinCells()
		{

			if (!ApplicationSettings.IsPinSet)
			{
				EnableDisablePinCell.Text = I18N.EnablePin;
				if (ActionSection.Contains(ChangePinCell))
				{
					ActionSection.Remove(ChangePinCell);
				}
			}
			else
			{
				EnableDisablePinCell.Text = I18N.DisablePin;
				if (!ActionSection.Contains(ChangePinCell))
				{
					ActionSection.Add(ChangePinCell);
				}
			}

			FingerprintCell.Switch.IsEnabled = ApplicationSettings.IsPinSet;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (!await CrossFingerprint.Current.IsAvailableAsync()) return;

			if (!Table.Root.Contains(FingerprintSection))
			{
				Table.Root.Add(FingerprintSection);
			}
		}

		private void EnableDisablePin(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new PinOverlay(PinAction.EnableOrDisable));
		}

		private void ChangePin(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new PinOverlay(PinAction.Change));
		}
	}
}
