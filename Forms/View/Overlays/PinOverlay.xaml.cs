using System;
using MyCC.Core.Settings;
using MyCC.Core.Types;
using MyCC.Forms.helpers;
using MyCC.Forms.Messages;
using MyCC.Forms.Resources;
using Xamarin.Forms;

namespace MyCC.Forms.view.overlays
{
    public partial class PinOverlay : ContentPage
    {
        private PinAction pinAction;

        public PinOverlay(PinAction pinAction)
        {
            InitializeComponent();

            if (pinAction == PinAction.EnableOrDisable)
            {
                this.pinAction = ApplicationSettings.IsPinSet ? PinAction.Disable : PinAction.Enable;
            }
            else
            {
                this.pinAction = pinAction;
            }

            OldPinCell.Entry.IsPassword = true;
            NewPinCell.Entry.IsPassword = true;
            NewPinRepeatCell.Entry.IsPassword = true;

            OldPinCell.Entry.Keyboard = Keyboard.Numeric;
            NewPinCell.Entry.Keyboard = Keyboard.Numeric;
            NewPinRepeatCell.Entry.Keyboard = Keyboard.Numeric;

            OldPinCell.Entry.TextChanged += (sender, e) =>
            {
                if (e.NewTextValue.Length != ApplicationSettings.PinLength) return;

                if (ApplicationSettings.IsPinValid(e.NewTextValue))
                {
                    if (this.pinAction == PinAction.Disable)
                    {
                        ApplicationSettings.Pin = null;
                        Messaging.Pin.SendValueChanged();
                        Navigation.PopOrPopModal();
                    }
                    else
                    {
                        NewPinCell.Entry.Focus();
                    }
                }
                else
                {
                    DisplayAlert(I18N.Error, I18N.OldPinWrong, I18N.Cancel);
                    OldPinCell.Entry.Text = string.Empty;
                }
            };

            Title = I18N.Pin;

            switch (this.pinAction)
            {
                case PinAction.Enable: Header.InfoText = I18N.EnablePin; PinTable.Root.Remove(OldPinSection); break;
                case PinAction.Disable: Header.InfoText = I18N.DisablePin; PinTable.Root.Remove(ChangePinSection); break;
                case PinAction.Change: Header.InfoText = I18N.ChangePin; break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (pinAction == PinAction.Enable)
            {
                NewPinCell.Entry.Focus();
            }
            else
            {
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

            var repeatOk = newPin.Equals(NewPinRepeatCell.Entry.Text) || pinAction == PinAction.Disable;
            var oldPinOk = ApplicationSettings.IsPinValid(oldPin) || pinAction == PinAction.Enable;
            var pinLongEnough = newPin.Length >= 4 || pinAction == PinAction.Disable;


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
            Messaging.Pin.SendValueChanged();
            Navigation.PopOrPopModal();
        }
    }
}