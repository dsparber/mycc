using System;
using data.settings;
using MyCryptos.enums;
using MyCryptos.helpers;
using MyCryptos.resources;
using Xamarin.Forms;

namespace MyCryptos.view.overlays
{
	public partial class PinOverlay : ContentPage
	{
		private PinAction pinAction;

		public PinOverlay(PinAction pinAction)
		{
			InitializeComponent();

			this.pinAction = pinAction;

			OldPinCell.Entry.IsPassword = true;
			NewPinCell.Entry.IsPassword = true;
			NewPinRepeatCell.Entry.IsPassword = true;

			OldPinCell.Entry.Keyboard = Keyboard.Numeric;
			NewPinCell.Entry.Keyboard = Keyboard.Numeric;
			NewPinRepeatCell.Entry.Keyboard = Keyboard.Numeric;

			OldPinCell.Entry.TextChanged += (sender, e) =>
			{
				if (e.NewTextValue.Length == ApplicationSettings.PinLength)
				{
					if (ApplicationSettings.IsPinValid(e.NewTextValue))
					{
						if (pinAction == PinAction.DISABLE)
						{
							ApplicationSettings.Pin = null;
							Navigation.PopOrPopModal();
						}
						else {
							NewPinCell.Entry.Focus();
						}
					}
					else {
						DisplayAlert(I18N.Error, I18N.OldPinWrong, I18N.Cancel);
						OldPinCell.Entry.Text = string.Empty;
					}
				}
			};

			switch (pinAction)
			{
				case PinAction.ENABLE: Title = I18N.EnablePin; PinTable.Root.Remove(OldPinSection); break;
				case PinAction.DISABLE: Title = I18N.DisablePin; PinTable.Root.Remove(ChangePinSection); break;
				case PinAction.CHANGE: Title = I18N.ChangePin; break;
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (pinAction == PinAction.ENABLE)
			{
				NewPinCell.Entry.Focus();
			}
			else {
				OldPinCell.Entry.Focus();
			}
		}

		void CancelClicked(object sender, EventArgs e)
		{
			Navigation.PopOrPopModal();
		}

		void SaveClicked(object sender, EventArgs e)
		{
			var oldPin = OldPinCell.Entry.Text ?? string.Empty;
			var newPin = NewPinCell.Entry.Text ?? string.Empty;

			var repeatOk = newPin.Equals(NewPinRepeatCell.Entry.Text) || pinAction == PinAction.DISABLE;
			var oldPinOk = ApplicationSettings.IsPinValid(oldPin) || pinAction == PinAction.ENABLE;
			var pinLongEnough = newPin.Length >= 4 || pinAction == PinAction.DISABLE;


			if (!oldPinOk)
			{
				DisplayAlert(I18N.Error, I18N.OldPinWrong, I18N.Cancel);
				return;
			}
			if (!pinLongEnough)
			{
				DisplayAlert(I18N.Error, I18N.PinToShort, I18N.Cancel);
				return;
			}
			if (!repeatOk)
			{
				DisplayAlert(I18N.Error, I18N.NewPinsDontMatch, I18N.Cancel);
				return;
			}

			ApplicationSettings.Pin = newPin;
			Navigation.PopOrPopModal();

		}
	}
}