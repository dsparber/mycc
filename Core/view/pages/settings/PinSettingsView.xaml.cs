using System;
using data.settings;
using message;
using MyCryptos.enums;
using MyCryptos.helpers;
using MyCryptos.view.overlays;
using Plugin.Fingerprint;
using Xamarin.Forms;

namespace MyCryptos.view.pages.settings
{
	public partial class PinSettingsView : ContentPage
	{
		public PinSettingsView()
		{
			InitializeComponent();

			SetPinCells();

			FingerprintCell.Switch.Toggled += (sender, e) => ApplicationSettings.IsFingerprintEnabled = e.Value;

			if (ApplicationSettings.IsFingerprintEnabled)
			{
				FingerprintCell.Switch.IsToggled = true;
			}
			else {
				Table.Root.Remove(FingerprintSection);
			}

			MessagingCenter.Subscribe<string>(this, MessageConstants.UpdatedPin, (str) => SetPinCells());
		}

		void SetPinCells()
		{
			ActionSection.Remove(EnablePinCell);
			ActionSection.Remove(DisablePinCell);
			ActionSection.Remove(ChangePinCell);

			if (!ApplicationSettings.IsPinSet)
			{
				ActionSection.Add(EnablePinCell);
				FingerprintCell.Switch.IsEnabled = false;
			}
			else {
				ActionSection.Add(DisablePinCell);
				ActionSection.Add(ChangePinCell);
				FingerprintCell.Switch.IsEnabled = true;
			}
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			if (await CrossFingerprint.Current.IsAvailableAsync())
			{
				if (!Table.Root.Contains(FingerprintSection))
				{
					Table.Root.Add(FingerprintSection);
				}
			}
		}

		void EnablePin(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new PinOverlay(PinAction.ENABLE));
		}

		void DisablePin(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new PinOverlay(PinAction.DISABLE));
		}

		void ChangePin(object sender, EventArgs e)
		{
			Navigation.PushOrPushModal(new PinOverlay(PinAction.CHANGE));
		}
	}
}
